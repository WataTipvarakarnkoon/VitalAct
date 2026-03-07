import 'package:flutter/material.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import '../../../models/steps/reading_step.dart';

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
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Column(
          children: [
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: SizedBox(
                  width: double.infinity, // force full width
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
                          style: const TextStyle(
                            fontSize: 18,
                            height: 1.6,
                            fontWeight: FontWeight.w600,
                            color: Color(0xFFA7A7A7),
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
