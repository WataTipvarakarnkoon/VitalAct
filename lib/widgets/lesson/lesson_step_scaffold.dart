import 'package:flutter/material.dart';
import 'lesson_button.dart';
import 'lesson_feedback_panel.dart';

class LessonStepScaffold extends StatelessWidget {
  final Widget child;

  final bool answered;
  final bool? isCorrect;

  final String? explanation;
  final String? hint;

  final String buttonText;
  final VoidCallback? onButtonPressed;

  const LessonStepScaffold({
    super.key,
    required this.child,
    required this.answered,
    required this.buttonText,
    this.isCorrect,
    this.explanation,
    this.hint,
    this.onButtonPressed,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Stack(
          children: [
            /// MAIN CONTENT
            child,

            /// FEEDBACK PANEL
            LessonFeedbackPanel(
              visible: answered,
              isCorrect: isCorrect ?? false,
              explanation: explanation,
              hint: hint,
            ),

            /// BUTTON
            Positioned(
              bottom: 0,
              child: LessonButton(
                text: buttonText,
                onPressed: onButtonPressed,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
