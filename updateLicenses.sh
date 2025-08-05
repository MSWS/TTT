# Credit: https://medium.com/@gareth.stretton/neovim-manipulate-markdown-tables-with-awk-7b7cc3b5f1e9
nuget-license -i TTT.sln -d licenses/ -o Markdown | awk -v OFS="|" '
BEGIN {
  FS = "|"
  for (i = 1; i < ARGC; i++) {
    columnId = ARGV[i]
    ARGV[i] = ""

    if (columnId ~ /^[0-9]+$/) {
      SKIP_COLUMN[columnId + 1] = 1
    } else {
      ARGV_STRING[tolower(columnId)] = 1
    }
  }
}

NR == 1 && length(ARGV_STRING) > 0 {
  for (i = 2; i < NF; i++) {
    if (ARGV_STRING[tolower(trim($i))] == 1) {
      SKIP_COLUMN[i] = 1
    }
  }
}

{
  line = ""
  for (i = 2; i < NF; i++) {
    if (SKIP_COLUMN[i] == 1) {
      continue;
    }
    line = line OFS $i
  }
  if (line != "") {
    printf("%s%s\n", line, OFS)
  }
}

function ltrim(s) { sub(/^[ \t\r\n]+/, "", s); return s }
function rtrim(s) { sub(/[ \t\r\n]+$/, "", s); return s }
function trim(s) { return rtrim(ltrim(s)) }
' 9 10 > LICENSES.MD
