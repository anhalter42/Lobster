rsync -v -r --delete ../../Unity/Lobster/Assets ./Assets
cp ../../Unity/Lobster/*.csproj .
cp ../../Unity/Lobster/*.sln .
cp -rf ../../Unity/Lobster/ProjectSettings ./ProjectSettings
rm -rf obj
rm -rf Library
rm -rf Temp
git add *
git commit -m "next `date`"
git push
