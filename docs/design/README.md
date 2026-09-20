# Interface design

The design for **PerformanceView**, and the deliverables for issue #6.

| File | What it is |
|---|---|
| `performance-view.html` | The working interface — open it in a browser. Responsive, both themes, all states implemented |
| `states.html` | Component state sheet — every control in default, hover, disabled and running |
| `figma-desktop.png` | The original Figma export this was built from |
| `export/desktop-1440.png` | Desktop layout, 1440 px |
| `export/mobile-375.png` | Phone layout, 375 px |
| `export/states-1440.png` | The state sheet |
| `logo/mark.svg` | Logo mark — descending bars |
| `img/` | Hero photograph and its licence, see `img/CREDITS.md` |

## Tokens

Sampled from the Figma export, not guessed.

| Role | Hex |
|---|---|
| Hero navy | `#21344B` |
| Magenta accent | `#FB47FF` |
| Green accent | `#38A07B` |
| Logo "View" | `#56A186` |
| Button gradient | `#EB50F2` → `#4C9788` |
| Page | `#F5F5F5` |
| Panel | `#E6E6E6` |
| Code block | `#F1F1F1` |

Type: **Poppins** for interface and display, **IBM Plex Mono** for SQL, metrics and plan operators.

## Layout

Artboard is **1440 px**. Nav 67 px, hero 365 px, columns **360 / fluid / 360** with 22 px gaps.

| Width | Layout |
|---|---|
| ≥ 1240 px | Three columns: schema · work · findings |
| 821–1240 px | Two columns, findings full width below |
| ≤ 820 px | One column. No expectations card. Order: query → findings → schema, folded |

## Regions

1. **Schema** — tables with row counts, expanding to columns with PK/FK marks
2. **Query** — editable SQL, or a plan-file drop zone, or a connection string, depending on source
3. **Duration** — duration, logical reads, rows returned, findings count
4. **Views** — execution plan · schema diagram · before & after
5. **Findings** — one card per rule, each with what · fix · cost · when not to

## Regenerating the exports

```bash
CHROME="/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
D=$(pwd)/docs/design

"$CHROME" --headless=new --disable-gpu --hide-scrollbars --window-size=1440,1500 \
  --screenshot="$D/export/desktop-1440.png" --virtual-time-budget=6000 \
  "file://$D/performance-view.html"
```

**Phone widths need a shim.** macOS enforces a minimum Chrome window of about 485 px, so a
`--window-size=375` screenshot silently renders at 485 and clips. Load the page in a 375 px iframe
and capture that instead:

```bash
printf '<!doctype html><style>html,body{margin:0}iframe{width:375px;height:2100px;border:0}</style><iframe src="file://%s/performance-view.html"></iframe>' "$D" > /tmp/shim.html
"$CHROME" --headless=new --disable-gpu --hide-scrollbars --window-size=375,2100 \
  --screenshot="$D/export/mobile-375.png" --virtual-time-budget=6000 "file:///tmp/shim.html"
```
