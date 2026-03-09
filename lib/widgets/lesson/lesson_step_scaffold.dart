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
      resizeToAvoidBottomInset: false,
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          /// MAIN CONTENT (safe from notch etc)
          SafeArea(
            child: child,
          ),

          /// FEEDBACK PANEL
          LessonFeedbackPanel(
            visible: answered,
            isCorrect: isCorrect ?? false,
            explanation: explanation,
            hint: hint,
          ),

          /// BUTTON (not protected by SafeArea)
          Positioned(
            left: 0,
            right: 0,
            bottom: MediaQuery.of(context).viewInsets.bottom == 0
                ? MediaQuery.of(context).padding.bottom
                : 0,
            child: LessonButton(
              text: buttonText,
              onPressed: onButtonPressed,
            ),
          )
        ],
      ),
    );
  }
}
