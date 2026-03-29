import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import 'package:vitalact/models/steps/reading_step.dart';

class ReadingPage extends StatelessWidget {
  final ReadingStep step;
  final VoidCallback onNext;

  const ReadingPage({
    super.key,
    required this.step,
    required this.onNext,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          children: [
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: SizedBox(
                  width: double.infinity,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        step.title,
                        style: const TextStyle(
                          fontSize: 26,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 16),
                      Text(step.content,
                          style: TextStyle(
                            fontSize: 18,
                            height: 1.6,
                            fontWeight: FontWeight.w600,
                            color: AppColors.textPrimary.withValues(alpha: 0.8),
                          )),
                      const SizedBox(height: 40),
                    ],
                  ),
                ),
              ),
            ),
            LessonButton(
              text: "Continue",
              onPressed: onNext,
            )
          ],
        ),
      ),
    );
  }
}
