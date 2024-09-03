# GalArc

Galgame Archive Tool.

Mainly focus on the unpacking and repacking of galgame archives.

## Supported Format

| Engine(Producer) | Extension                   | Unpack | Pack | Supported                        |
| ---------------- | --------------------------- | ------ | ---- | -------------------------------- |
| AdvHD            | `.arc` `.pna`               | ✔️      | ✔️    | All                              |
| Ai5Win           | `.VSD`                      | ✔️      | ❌    | All                              |
| AmuseCraft       | `.pac`                      | ✔️      | ✔️    | All                              |
| Artemis          | `.pfs`                      | ✔️      | ✔️    | All                              |
| EntisGLS         | `.noa`                      | ✔️      | ✔️    | Only unencrypted                 |
| InnocentGray     | `.iga` `.dat`               | ✔️      | ✔️    | `iga`:All `dat`:Limited          |
| KID              | `.dat`                      | ✔️      | ✔️    | All                              |
| NextonLikeC      | `.lst` `(empty)`            | ✔️      | ❌    | All                              |
| Nitro+           | `.pak`                      | ✔️      | ✔️    | All                              |
| NScripter        | `.ns2` `.dat`               | ✔️      | ✔️    | `ns2`:Only unencrypted `dat`:All |
| Silky            | `.arc`                      | ✔️      | ❌    | All                              |
| SystemNNN        | `.gpk` `.gtb` `.vpk` `.vtb` | ✔️      | ✔️    | All                              |
| Triangle         | `.CG` `.SUD`                | ✔️      | ✔️    | All                              |
