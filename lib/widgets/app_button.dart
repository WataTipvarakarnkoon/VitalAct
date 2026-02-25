import 'package:flutter/material.dart';

enum ButtonVariant { filled, outlined }

class AppButton extends StatelessWidget {
  final String text;
  final VoidCallback onPressed;
  final ButtonVariant variant;
  final double? width;
  final double height;

  const AppButton({
    super.key,
    required this.text,
    required this.onPressed,
    this.variant = ButtonVariant.filled,
    this.width,
    this.height = 60,
  });

  @override
  Widget build(BuildContext context) {
    final isFilled = variant == ButtonVariant.filled;

    return SizedBox(
      width: width ?? MediaQuery.of(context).size.width * 0.9,
      height: height,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(50),
          boxShadow: isFilled
              ? [
                  BoxShadow(
                    color: Color(0xCCCC3838).withValues(alpha: 40),
                    blurRadius: 0,
                    offset: Offset(0, 5),
                  ),
                ]
              : [],
        ),

        child: ElevatedButton(
          onPressed: onPressed,
          style: ElevatedButton.styleFrom(
            backgroundColor: isFilled ? const Color(0xFFFF4646) : Colors.white,
            foregroundColor: isFilled ? Colors.white : const Color(0xFFFF4646),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(50),
              side: isFilled
                  ? BorderSide.none
                  : const BorderSide(color: Color(0xFFFF9393), width: 4),
            ),
          ),
          child: Text(
            text,
            style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 20),
          ),
        ),
      ),
    );
  }
}
