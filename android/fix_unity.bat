@echo off
echo Fixing unityLibrary build.gradle...

powershell -Command "(gc unityLibrary\build.gradle) -replace 'commandLineArgs.add\(\"--tool-chain-path=\" \+ android.ndkDirectory\)', 'commandLineArgs.add(\"--tool-chain-path=C:/Program Files/Unity/Hub/Editor/2022.3.62f3/Editor/Data/PlaybackEngines/AndroidPlayer/NDK\")' | Out-File -encoding ASCII unityLibrary\build.gradle"

powershell -Command "(gc unityLibrary\build.gradle) -replace 'ndkPath.*', '' | Out-File -encoding ASCII unityLibrary\build.gradle"

echo Done! Now run flutter build apk