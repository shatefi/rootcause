## 2026-09-21 — attached the Stack Overflow database

- Attached the Mini 2013 dataset to SQL Server 2025. It is **five** files, not two — `_1.mdf`
plus `_2/_3/_4.ndf` plus the log. Every attach example online shows only mdf+ldf, so all five
have to be listed explicitly in `CREATE DATABASE … FOR ATTACH` or it fails.
- Surprise: it attached at **compatibility level 100**, not 170. The dataset is in SQL Server 2008
format, and 2025 still supports level 100, so nothing raised it. That is exactly the case this
whole project is about — a modern engine optimising like 2008 because nobody changed the level
after a restore. My own test data is the demo.

