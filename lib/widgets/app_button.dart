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
                  const BoxShadow(
                    color: Color(0xFFCC3838),
                    offset: Offset(0, 5),
                  ),
                ]
              : [],
        ),
        child: OutlinedButton(
          onPressed: onPressed,
          style: OutlinedButton.styleFrom(
            backgroundColor: isFilled ? const Color(0xFFFF4646) : Colors.white,
            foregroundColor: isFilled ? Colors.white : const Color(0xFFFF4646),
            side: BorderSide(
              color:
                  isFilled ? const Color(0xFFCC3838) : const Color(0xFFFF9393),
              width: isFilled ? 2.0 : 3.0,
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
