rsync -v -r --delete ../../Unity/Lobster/Assets .
cp ../../Unity/Lobster/*.csproj .
cp ../../Unity/Lobster/*.sln .
rsync -v -r --delete ../../Unity/Lobster/ProjectSettings .
rm -rf obj
rm -rf Library
rm -rf Temp
git add *
git commit -m "next `date`"
git push
