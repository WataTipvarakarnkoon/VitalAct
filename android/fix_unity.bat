@echo off
echo Fixing unityLibrary after Unity export...

set GRADLE=D:\Code Stuffs\VSCode Study\Flutter\Project\VitalAct\android\unityLibrary\build.gradle

echo Removing mobilenotifications.androidlib dependency...
powershell -Command "(gc '%GRADLE%') -replace \".*implementation project\('mobilenotifications.androidlib'\).*\", '' | Out-File -encoding ASCII '%GRADLE%'"

echo Removing ndkPath line...
powershell -Command "(gc '%GRADLE%') -replace '.*ndkPath.*', '' | Out-File -encoding ASCII '%GRADLE%'"

echo Fixing IL2CPP toolchain path...
powershell -Command "(gc '%GRADLE%') -replace 'commandLineArgs\.add\(""--tool-chain-path="" \+ android\.ndkDirectory\)', 'commandLineArgs.add(""--tool-chain-path=C:/Program Files/Unity/Hub/Editor/2022.3.62f3/Editor/Data/PlaybackEngines/AndroidPlayer/NDK"")' | Out-File -encoding ASCII '%GRADLE%'"

echo Copying libc++_shared.so (required by MediaPipe)...
copy /Y "C:\Users\mrpru\AppData\Local\Android\Sdk\ndk\26.1.10909125\toolchains\llvm\prebuilt\windows-x86_64\sysroot\usr\lib\aarch64-linux-android\libc++_shared.so" "D:\Code Stuffs\VSCode Study\Flutter\Project\VitalAct\android\unityLibrary\src\main\jniLibs\arm64-v8a\libc++_shared.so"

echo All fixes applied!
echo You can now run: flutter build apk
pause