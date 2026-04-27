This application is a derivitive of the larger project 'SQLite Insights', currently under development.

# What is an SQLite header?

From the documentation (https://sqlite.org/fileformat.html):

*The first 100 bytes of the database file comprise the database file header. The database file header is divided into fields as shown by the table below. All multibyte fields in the database file header are stored with the most significant byte first (big-endian).*


|Offset|Size|Description|
|-|-|-|
|0|16|The header string: "SQLite format 3\000"|
|16|2|The database page size in bytes. Must be a power of two between 512 and 32768 inclusive, or the value 1 representing a page size of 65536.|
|18|1|File format write version. 1 for legacy; 2 for WAL.|
|19|1|File format read version. 1 for legacy; 2 for WAL.|
|20|1|Bytes of unused "reserved" space at the end of each page. Usually 0.|
|21|1|Maximum embedded payload fraction. Must be 64.|
|22|1|Minimum embedded payload fraction. Must be 32.|
|23|1|Leaf payload fraction. Must be 32.|
|24|4|File change counter.|
|28|4|Size of the database file in pages. The "in-header database size".|
|32|4|Page number of the first freelist trunk page.|
|36|4|Total number of freelist pages.|
|40|4|The schema cookie.|
|44|4|The schema format number. Supported schema formats are 1, 2, 3, and 4.|
|48|4|Default page cache size.|
|52|4|The page number of the largest root b-tree page when in auto-vacuum or incremental-vacuum modes, or zero otherwise.|
|56|4|The database text encoding. A value of 1 means UTF-8. A value of 2 means UTF-16le. A value of 3 means UTF-16be.|
|60|4|The "user version" as read and set by the user_version pragma.|
|64|4|True (non-zero) for incremental-vacuum mode. False (zero) otherwise.|
|68|4|The "Application ID" set by PRAGMA application_id.|
|72|20|Reserved for expansion. Must be zero.|
|92|4|The version-valid-for number.|
|96|4|SQLITE_VERSION_NUMBER|

<br>

**An explanation of these fields is provided in the documentation link.**

<br>

# Forensic Application
I believe the following are '*must checks*' each time I examine an SQLite database outside of a Forensic software before conducting any analysis:

* **Page Size** - This tells us our page boundaries. As SQLite is built up of fixed page sizes, it is essential to know this when manually parsing, or carving records. For example, some SQLite records can overflow a page. Not knowing the page size can result in a failed carve or parse event.

* **File Change Counter** - This tells us how many times a '*commit*' has occurred on this database. It is an incremental counter. This is especially useful for scenarios where a database is empty of user data, and you are unsure if the application has been used or not. Be aware that when an application makes a database, it will conduct at least 1 commit to insert a table(s). For example, with Chrome, you may see 3 to 5 commits before the user has begun to add data. The VACUUM processes do not reset this counter (as of 2026/04/27).

* **Page Number of the First Freelist Trunk Page.** - Exactly what it says. It is the page number of the first Freelist page. This is the root page number, which is not the offset. To get to an overall page offset, you must use the calculation ```"(pageSize * PageNumber) - pageSize"```. The first part of this formula provides an offset to the bottom of the SQLite target page. Minus 1 page size just takes us to the top of the page.

* **Page Number of the Largest Root B-Tree Page** - This tells us if the database has Auto-Vacuum enabled. Offset 64 will then tell us if the database Auto-Vacuum mode is set to 'Incremental', or 'Full'.

Whilst you can use the other fields as and when needed, I believe the above are essential.

**As a final note on pertinent fields:** Common forensic tools are using SQLite frameworks to access SQLite databases. This involves reading the database using a programming library. So what would happen if I were to change offset 19 (the read flag) from a value other than 1 or 2? The forensic application will not read the database, and only carve data from the database. Some forensic tools are better than others at SQLite carving, but major vendors are not carving correctly, resulting in missed data. If you find a database has no live data, but all carved, it is worth manually checking the database for the read flag status. It should be 1 or 2, and match the write flag. 1 = Rollback Journal. 2 = Write-Ahead Log (WAL).

<br>

# Final Note: Vacuuming Types
This seems to be an area with some amount of misinformation in the field. There are two (2) overarching types of vacuuming: **VACUUM (the pragma) and AUTO-VACUUM**. I would advise to not fall into this misinformation trap, as there are a few in the industry a lot of us fall for (for example outside of SQLite - yes, you can still do a RAM capture of a laptop even when it has been powered down and get data back from the previous sessions).

### VACUUM
This mode pertains to the PRAGMA what rebuilds the database into a new file. It *defragments* the database, rebuilding the B-Tree and removes all non-live data. Any non-live data is lost when the process happens. This can result in a smaller, quicker and better structured database.

<br>

### AUTO-VACUUM
This comes in two (2) modes which work by truncating **Freelist** pages from the database. Truncated pages are lost when the process happens. It is an automated process what occurs when a trigger is satisfied. Our 2 modes are:

* FULL (1): Actively moves pages to truncate the file. This triggers when a **COMMIT** action occurs.

* INCREMENTAL (2): Like FULL, but it does not truncate the file immediately. Instead, it maintains a list of pages to be moved, and you must call PRAGMA incremental_vacuum(N) to truncate N pages at a time. This allows you to manage the I/O impact.

<br>

### So how does a page end up in the Freelist?

A page is only added to the freelist when it ceases to be a functional member of the B-Tree hierarchy. This usually happens in one of two ways: during a deletion process or during a tree-balancing operation.

A page becomes a candidate for the freelist only when the cell count (the number of records stored on it) drops to zero.

**Row Deletion:** When you execute a DELETE statement, the *Virtual Database Engine* (VDBE) removes the corresponding cell from the page's cell content area and updates the cell pointer array (i.e., the counter). If that deletion causes the record count on the page to hit zero, the page is now empty.

**Tree Rebalancing:** SQLite maintains B-Tree balance. If deleting a record or moving data causes a parent page to have only one child, or causes a leaf page to become empty, the B-Tree logic will perform a "merge" or "rebalance." During this restructuring, pages that are no longer needed are unlinked from the tree and added to the freelist.

<br>

### Tree rebalancing is a phenomenon whereby common forensic tools may not differentiate between the live and the deleted record. But why?

When a page becomes too sparse or too full, the B-Tree balancer may decide to move records between pages. Here is the sequence of events that results in a "ghost" of a record persisting in the freelist while the "live" version exists elsewhere:

* **The Move:** Imagine a record $R$ currently exists on Page $A$. The B-Tree balancer determines that Page $A$ needs to be reorganized or merged.

* **The Copy:** The VDBE executes an instruction to move the content of Page $A$ to a new page, Page $B$ (or a new position within the tree). The data for record $R$ is copied into the cell structure of the new page.

* **The Original Deletion:** Because the data has been successfully moved to the "live" location (Page $B$), the engine effectively deletes the record from the original Page $A$ to reclaim space or complete the move. Remember, SQLite by default does not *zero* a record, or the pointer in the cell pointer array. *The PRAGMA **SECURE_DELETE** is used for 'forensically' wiping a record upon deletion (zeroed out), like what we see in Chromium stype databases (which is why we cannot recover deleted web history, contrary to popular belief, and only recover records which are live or in the freelist).*

* **Freelist Enlistment:** If the move leaves Page $A$ with zero records, the Pager marks Page $A$ as free and adds it to the freelist.

**The Result:** The bits representing record $R$ still reside in the physical storage of the page that was just moved to the freelist. Simultaneously, the identical record $R$ is now "live" on the new page.

This essentially means that even if **AUTO_VACUUM** has been used, there is still an opportunity to recover deleted records, if it has not been moved to the Freelist and truncated. If a user deleted a full set of WhatsApp messages (i.e., a thread), you may find that the VDBE has rebalanced the database which would result in the deletions being truncated. This points out the value of turning off a device as soon as possible to retain that data (as it typically will happen during standby events where the device / app is not in use).
