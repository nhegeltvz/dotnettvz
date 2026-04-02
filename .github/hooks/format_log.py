import json

LOG = 'C:/Users/nikol/Code/dotnettvz/.github/hooks/agent_log.txt'

entries = []
with open(LOG, encoding='utf-8', errors='replace') as f:
    for line in f:
        line = line.strip()
        if line:
            try:
                entries.append(json.loads(line))
            except Exception:
                entries.append(line)  # keep unparseable lines as-is

formatted = []
for entry in entries:
    if isinstance(entry, dict):
        formatted.append(json.dumps(entry, indent=2, ensure_ascii=False))
    else:
        formatted.append(entry)

with open(LOG, 'w', encoding='utf-8') as f:
    f.write('\n\n'.join(formatted) + '\n')

print(f'Formatted {len(entries)} entries.')
