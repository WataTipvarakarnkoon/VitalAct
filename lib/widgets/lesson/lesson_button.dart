import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';

enum LessonButtonType { continueButton, option }

class LessonButton extends StatelessWidget {
  final VoidCallback? onPressed;
  final String text;

  final LessonButtonType type;

  // option mode only
  final bool selected;

  const LessonButton({
    super.key,
    required this.text,
    this.onPressed,
    this.type = LessonButtonType.continueButton,
    this.selected = false,
  });

  @override
  Widget build(BuildContext context) {
    final bool enabled = onPressed != null;

    if (type == LessonButtonType.option) {
      return AppButton(
        height: 56,
        onPressed: onPressed,
        borderRadius: 15,
        variant: selected ? ButtonVariant.filled : ButtonVariant.outlined,
        borderColor:
            selected ? const Color(0xFFC42F2F) : const Color(0xFFD0D0D0),
        backgroundColor: selected ? const Color(0xFFFF4646) : Colors.white,
        borderWidth: 3,
        child: Text(
          text,
          textAlign: TextAlign.center,
          style: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 18,
            color: selected ? Colors.white : const Color(0xFF7C7C7C),
          ),
        ),
      );
    }

    // CONTINUE BUTTON MODE
    return Padding(
      padding: const EdgeInsets.all(20),
      child: AppButton(
        borderRadius: 15,
        height: 50,
        onPressed: onPressed,
        borderColor:
            enabled ? const Color(0xFFCC3838) : const Color(0xFF8A8A8A),
        backgroundColor:
            enabled ? const Color(0xFFFF4646) : const Color(0xFFBDBDBD),
        foregroundColor: Colors.white,
        child: Text(
          text,
          style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
