rsync -r ../../Unity/Lobster/ .
rm -rf obj
rm -rf Library
git add *
git commit -m next
git push
