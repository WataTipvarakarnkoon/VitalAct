import 'package:flutter/material.dart';

enum ButtonVariant { filled, outlined }

class AppButton extends StatelessWidget {
  final Widget child;
  final VoidCallback onPressed;
  final ButtonVariant variant;
  final double? width;
  final double height;

  final Color? backgroundColor;
  final Color? foregroundColor;
  final Color? borderColor;
  final Color? shadowColor;

  final double borderRadius;

  final EdgeInsetsGeometry padding;

  const AppButton({
    super.key,
    required this.child,
    required this.onPressed,
    this.variant = ButtonVariant.filled,
    this.width,
    this.height = 60,
    this.backgroundColor,
    this.foregroundColor,
    this.borderColor,
    this.shadowColor,
    this.borderRadius = 50,
    this.padding = const EdgeInsets.symmetric(),
  });

  @override
  Widget build(BuildContext context) {
    final isFilled = variant == ButtonVariant.filled;

    final defaultBackground = isFilled ? const Color(0xFFFF4646) : Colors.white;

    final defaultForeground = isFilled ? Colors.white : const Color(0xFFFF4646);

    final defaultBorder =
        isFilled ? const Color(0xFFCC3838) : const Color(0xFFFF9393);

    const defaultShadow = Color(0xFFCC3838);

    return SizedBox(
      width: width ?? MediaQuery.of(context).size.width * 0.9,
      height: height,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(borderRadius),
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
            padding: padding,
            backgroundColor: backgroundColor ?? defaultBackground,
            foregroundColor: foregroundColor ?? defaultForeground,
            side: BorderSide(
              color: borderColor ?? defaultBorder,
              width: isFilled ? 2.0 : 3.0,
            ),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(borderRadius),
            ),
          ),
          child: DefaultTextStyle(
            style: TextStyle(
              fontFamily: 'BalooBhai2',
              fontSize: 22,
              fontWeight: FontWeight.w700,
              color: foregroundColor ?? defaultForeground,
            ),
            child: child,
          ),
        ),
      ),
    );
  }
}
