import re, sys, io, pathlib
csproj = pathlib.Path(sys.argv[1]); props = pathlib.Path(sys.argv[2])
s = csproj.read_text(encoding="utf-8-sig")
found = re.findall(r'<PackageReference\s+Include="([^"]+)"\s+Version="([^"]+)"', s)
csproj.write_text(re.sub(r'(<PackageReference\s+Include="[^"]+")\s+Version="[^"]+"', r'\1', s), encoding="utf-8")
p = props.read_text(encoding="utf-8-sig")
add = "".join(f'    <PackageVersion Include="{i}" Version="{v}" />\n'
               for i, v in found if f'Include="{i}"' not in p)
if add: p = p.replace("  </ItemGroup>", add + "  </ItemGroup>", 1)
props.write_text(p, encoding="utf-8")
print(f"moved {len(found)} package version(s):", ", ".join(i for i, _ in found))
