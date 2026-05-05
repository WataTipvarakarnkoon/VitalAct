import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import 'package:vitalact/widgets/sprite_animation.dart';
import 'package:vitalact/screens/practice/cpr_placement_test_page.dart';
import 'package:vitalact/l10n/app_localizations.dart';

class CprPlacementReadingPage extends StatelessWidget {
  const CprPlacementReadingPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        backgroundColor: AppColors.background,
        surfaceTintColor: AppColors.background,
        leading: IconButton(
          padding: const EdgeInsets.only(bottom: 2),
          icon: const Icon(Icons.clear_rounded, color: AppColors.disabled),
          onPressed: () => Navigator.pop(context),
        ),
        title: Text(
          l10n.cprPlacementTitle,
          style: const TextStyle(
            fontSize: 20,
            color: AppColors.disabled,
            fontWeight: FontWeight.w500,
          ),
        ),
        centerTitle: true,
        bottom: const PreferredSize(
          preferredSize: Size.fromHeight(1),
          child: Divider(color: AppColors.disabled, height: 1, thickness: 1),
        ),
      ),
      body: SafeArea(
        child: Column(
          children: [
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const SizedBox(height: 24),
                    Text(
                      l10n.cprPlacementReadingTitle,
                      style: const TextStyle(
                        fontSize: 26,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      l10n.cprPlacementReadingDetail,
                      style: TextStyle(
                        fontSize: 18,
                        height: 1.6,
                        fontWeight: FontWeight.w600,
                        color: AppColors.textPrimary.withValues(alpha: 0.8),
                      ),
                    ),
                    const SizedBox(height: 28),
                    Center(
                      child: SizedBox(
                        height: 260,
                        child: SpriteSheet(
                          asset: 'assets/spritesheet/CPRLocation.png',
                          columns: 40,
                          rows: 1,
                          totalFrames: 40,
                          fps: 30,
                          height: 256,
                          width: 256,
                        ),
                      ),
                    ),
                    const SizedBox(height: 24),
                    Text(
                      l10n.cprPlacementReadingCompression,
                      style: TextStyle(
                        fontSize: 17,
                        height: 1.6,
                        fontWeight: FontWeight.w600,
                        color: AppColors.textPrimary.withValues(alpha: 0.8),
                      ),
                    ),
                    const SizedBox(height: 40),
                  ],
                ),
              ),
            ),
            LessonButton(
              text: l10n.startTest,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) => const CprPlacementTestPage(),
                  ),
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}
