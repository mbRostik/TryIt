text = "Hello\u200B, my\u202Fname\u200D is Rostyslav."

for i, char in enumerate(text):
    code = ord(char)
    if code in [0x200B, 0x202F, 0x200D]:
        print(f"Index {i}: U+{code:04X} ({repr(char)})")