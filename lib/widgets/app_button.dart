import 'package:flutter/material.dart';

enum ButtonVariant { filled, outlined }

enum Shadowstyle { gray, red }

class AppButton extends StatelessWidget {
  final Widget child;
  final bool Dropshadow;
  final VoidCallback? onPressed;
  final ButtonVariant variant;
  final double? width;
  final double height;
  final double? borderWidth;

  final Color? backgroundColor;
  final Color? foregroundColor;
  final Color? borderColor;

  final double borderRadius;
  final EdgeInsetsGeometry padding;

  const AppButton({
    super.key,
    required this.child,
    required this.onPressed,
    this.variant = ButtonVariant.filled,
    this.Dropshadow = false,
    this.width,
    this.height = 60,
    this.backgroundColor,
    this.foregroundColor,
    this.borderColor,
    this.borderRadius = 50,
    this.padding = const EdgeInsets.symmetric(),
    this.borderWidth,
  });

  @override
  Widget build(BuildContext context) {
    final isFilled = variant == ButtonVariant.filled;

    final defaultBackground = isFilled ? const Color(0xFFFF4646) : Colors.white;
    final defaultForeground = isFilled ? Colors.white : const Color(0xFFFF4646);
    final defaultBorder =
        isFilled ? const Color(0xFFCC3838) : const Color(0xFFFF9393);
    const defaultBorderWidth = 3.0;

    return SizedBox(
      width: width ?? MediaQuery.of(context).size.width * 0.9,
      height: height,
      child: Container(
        decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(borderRadius),
            boxShadow: [
              Dropshadow
                  ? const BoxShadow(
                      color: Color.fromARGB(56, 128, 0, 0),
                      offset: Offset(6, 6),
                      blurRadius: 5)
                  : const BoxShadow(
                      color: Color.fromARGB(25, 0, 0, 0),
                      offset: Offset(6, 6),
                      blurRadius: 4)
            ]),
        child: Stack(
          children: [
            Positioned.fill(
              child: OutlinedButton(
                onPressed: onPressed,
                style: OutlinedButton.styleFrom(
                  padding: padding,
                  backgroundColor: backgroundColor ?? defaultBackground,
                  foregroundColor: foregroundColor ?? defaultForeground,
                  side: BorderSide(
                    color: isFilled
                        ? Colors.transparent
                        : (borderColor ?? defaultBorder),
                    width: borderWidth ?? defaultBorderWidth,
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

            // Inner shadow overlay (filled variant only)
            if (isFilled)
              Positioned.fill(
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(borderRadius),
                  child: IgnorePointer(
                    child: Stack(
                      children: [
                        Container(
                          decoration: const BoxDecoration(
                            gradient: LinearGradient(
                              begin: Alignment.centerRight,
                              end: Alignment.centerLeft,
                              colors: [
                                Color.fromARGB(61, 0, 0, 0),
                                Color(0x00000000)
                              ],
                              stops: [0.0, 0.075],
                            ),
                          ),
                        ),
                        Container(
                          decoration: const BoxDecoration(
                            gradient: LinearGradient(
                              begin: Alignment.bottomCenter,
                              end: Alignment.topCenter,
                              colors: [
                                Color.fromARGB(57, 0, 0, 0),
                                Color(0x00000000)
                              ],
                              stops: [0.0, 0.3],
                            ),
                          ),
                        ),
                        Container(
                          decoration: const BoxDecoration(
                            gradient: LinearGradient(
                              begin: Alignment.topCenter,
                              end: Alignment.bottomCenter,
                              colors: [
                                Color.fromARGB(62, 255, 255, 255),
                                Color.fromARGB(0, 255, 255, 255),
                              ],
                              stops: [0.0, 0.3],
                            ),
                          ),
                        ),
                        Container(
                          decoration: const BoxDecoration(
                            gradient: LinearGradient(
                              begin: Alignment.centerLeft,
                              end: Alignment.centerRight,
                              colors: [
                                Color.fromARGB(62, 255, 255, 255),
                                Color.fromARGB(0, 255, 255, 255),
                              ],
                              stops: [0.0, 0.06],
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
