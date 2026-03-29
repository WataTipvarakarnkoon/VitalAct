import 'package:flutter/material.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/theme/app_colors.dart';

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
        dropShadow: false,
        height: 56,
        onPressed: onPressed,
        borderRadius: 15,
        variant: selected ? ButtonVariant.filled : ButtonVariant.outlined,
        borderColor: AppColors.border, // only for outlined variant
        backgroundColor: selected ? AppColors.primary : AppColors.background,
        borderWidth: 3,
        child: Text(
          text,
          textAlign: TextAlign.center,
          style: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 18,
            color: selected ? AppColors.background : AppColors.textPrimary,
          ),
        ),
      );
    }

    // CONTINUE BUTTON MODE
    return Padding(
      padding: const EdgeInsets.all(20),
      child: AppButton(
        dropShadow: false,
        borderRadius: 15,
        height: 50,
        onPressed: onPressed,
        backgroundColor: enabled ? AppColors.primary : AppColors.disabled,
        foregroundColor: AppColors.background,
        child: Text(
          text,
          style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}
