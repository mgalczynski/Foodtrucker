FILES=*.md
for f in $FILES
do
  # extension="${f##*.}"
  filename="${f%.*}"
  echo "Converting $f to $filename.pdf"
  `pandoc $f --pdf-engine=xelatex -o $filename.pdf`
  # uncomment this line to delete the source file.
  # rm $f
done