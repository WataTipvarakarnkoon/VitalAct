import 'package:flutter/material.dart';
import 'screens/splash_screen.dart';
import 'screens/main/index_page.dart';
import 'screens/auth/auth_gate.dart';
import 'firebase_options.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/services.dart';
import 'l10n/app_localizations.dart';

final appLocale = ValueNotifier<Locale>(const Locale('en'));

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Set the system UI mode to hide the status bar
  SystemChrome.setEnabledSystemUIMode(SystemUiMode.manual, overlays: [
    SystemUiOverlay.bottom
  ]); // Hides status bar, keeps bottom nav bar

  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ValueListenableBuilder<Locale>(
      valueListenable: appLocale,
      builder: (context, locale, _) {
        return MaterialApp(
          locale: locale,
          theme: ThemeData(
            fontFamily: 'BalooBhai2',
          ),
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          debugShowCheckedModeBanner: false,
          home: const AuthGate(),
          routes: {
            '/splash': (context) => const SplashScreen(),
            '/index': (context) => const IndexPage(),
          },
        );
      },
    );
  }
}
