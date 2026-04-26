import 'package:flutter/material.dart';
import 'package:vitalact/l10n/app_localizations.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/sprite_animation.dart';

class WelcomePage extends StatelessWidget {
  final VoidCallback onLogin;
  final VoidCallback onSignup;

  const WelcomePage({
    super.key,
    required this.onLogin,
    required this.onSignup,
  });

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Scaffold(
      backgroundColor: AppColors.background,
      body: Stack(
        children: [
          Positioned(
            child: Image.asset(
              'assets/images/Background.png',
              fit: BoxFit.cover,
              width: width,
              height: height * 0.7,
            ),
          ),
          Positioned(
            top: height * -0.15,
            left: width * -0.6,
            child: Image.asset(
              'assets/images/Ellipse.png',
              height: height * 0.65,
            ),
          ),
          Positioned(
              top: height * 0.21,
              left: 0,
              right: 0,
              child: const SpriteSheet(
                asset: "assets/spritesheet/wave.png",
                columns: 60,
                rows: 1,
                totalFrames: 60,
                fps: 30,
                width: 512,
                height: 512,
              )),
          Positioned(
            left: 0,
            right: 0,
            top: height * 0.1,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  AppLocalizations.of(context)!.appName,
                  style: const TextStyle(
                    fontSize: 70,
                    fontWeight: FontWeight.w800,
                    color: AppColors.background,
                    height: 0.65,
                  ),
                ),
                const SizedBox(height: 17),
                Text(
                  AppLocalizations.of(context)!.tagline,
                  style: const TextStyle(
                    fontSize: 25,
                    fontWeight: FontWeight.w700,
                    color: AppColors.background,
                    height: 0.65,
                  ),
                ),
              ],
            ),
          ),
          Align(
            alignment: const Alignment(0, .75),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                AppButton(
                  dropShadow: true,
                  width: width * 0.9,
                  onPressed: onSignup,
                  height: 60,
                  child: Text(
                      AppLocalizations.of(context)!.getStarted.toUpperCase()),
                ),
                const SizedBox(height: 23),
                AppButton(
                  dropShadow: true,
                  width: width * 0.9,
                  variant: ButtonVariant.outlined,
                  onPressed: onLogin,
                  height: 60,
                  child: Text(AppLocalizations.of(context)!
                      .alreadyHaveAccount
                      .toUpperCase()),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
