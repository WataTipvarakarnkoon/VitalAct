import 'package:flutter/material.dart';

enum ButtonVariant { filled, outlined }

class AppButton extends StatelessWidget {
  final String text;
  final VoidCallback onPressed;
  final ButtonVariant variant;
  final double? width;
  final double height;

  // Optional color overrides
  final Color? backgroundColor;
  final Color? foregroundColor;
  final Color? borderColor;
  final Color? shadowColor;

  const AppButton({
    super.key,
    required this.text,
    required this.onPressed,
    this.variant = ButtonVariant.filled,
    this.width,
    this.height = 60,
    this.backgroundColor,
    this.foregroundColor,
    this.borderColor,
    this.shadowColor,
  });

  @override
  Widget build(BuildContext context) {
    final isFilled = variant == ButtonVariant.filled;

    final Color defaultBackground =
        isFilled ? const Color(0xFFFF4646) : Colors.white;

    final Color defaultForeground =
        isFilled ? Colors.white : const Color(0xFFFF4646);

    final Color defaultBorder =
        isFilled ? const Color(0xFFCC3838) : const Color(0xFFFF9393);

    const Color defaultShadow = Color(0xFFCC3838);

    return SizedBox(
      width: width ?? MediaQuery.of(context).size.width * 0.9,
      height: height,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(50),
          boxShadow: isFilled
              ? [
                  BoxShadow(
                    color: shadowColor ?? defaultShadow,
                    offset: const Offset(0, 5),
                  ),
                ]
              : [],
        ),
        child: OutlinedButton(
          onPressed: onPressed,
          style: OutlinedButton.styleFrom(
            backgroundColor: backgroundColor ?? defaultBackground,
            foregroundColor: foregroundColor ?? defaultForeground,
            side: BorderSide(
              color: borderColor ?? defaultBorder,
              width: isFilled ? 2.0 : 3.0,
            ),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(50),
            ),
          ),
          child: Text(
            text,
            style: const TextStyle(
              fontWeight: FontWeight.w700,
              fontSize: 20,
            ),
          ),
        ),
      ),
    );
  }
}
