rsync -r ../../Unity/Lobster/ .
rm -rf obj
rm -rf Library
rm -rf Temp
git add *
git commit -m "next `date`"
git push
