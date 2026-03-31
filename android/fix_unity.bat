@echo off
echo Fixing unityLibrary after Unity export...

set GRADLE=D:\Code Stuffs\VSCode Study\Flutter\Project\VitalAct\android\unityLibrary\build.gradle

echo Removing mobilenotifications.androidlib dependency...
powershell -Command "(gc '%GRADLE%') -replace \".*implementation project\('mobilenotifications.androidlib'\).*\", '' | Out-File -encoding ASCII '%GRADLE%'"

echo Removing ndkPath line...
powershell -Command "(gc '%GRADLE%') -replace '.*ndkPath.*', '' | Out-File -encoding ASCII '%GRADLE%'"

echo Fixing IL2CPP toolchain path...
powershell -Command "(gc '%GRADLE%') -replace 'commandLineArgs\.add\(""--tool-chain-path="" \+ android\.ndkDirectory\)', 'commandLineArgs.add(""--tool-chain-path=C:/Program Files/Unity/Hub/Editor/2022.3.62f3/Editor/Data/PlaybackEngines/AndroidPlayer/NDK"")' | Out-File -encoding ASCII '%GRADLE%'"

echo All fixes applied!
echo You can now run: flutter build apk
pause