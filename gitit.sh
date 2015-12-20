rsync -r ../../Unity/Lobster/ .
rm -rf obj
git add *
git commit -m next
git push
