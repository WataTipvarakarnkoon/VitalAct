import 'package:flutter/material.dart';
import 'dart:math';

class LessonFeedbackPanel extends StatelessWidget {
  final bool visible;
  final bool isCorrect;
  final String? explanation;
  final String? hint;

  const LessonFeedbackPanel({
    super.key,
    required this.visible,
    required this.isCorrect,
    this.explanation,
    this.hint,
  });

  @override
  Widget build(BuildContext context) {
    final bottomSafe = MediaQuery.of(context).padding.bottom;

    final Color background =
        isCorrect ? const Color(0xFFDFF6DD) : const Color(0xFFFFE5E5);

    final Color accent =
        isCorrect ? const Color(0xFF2E7D32) : const Color(0xFFF83B3B);

    return AnimatedPositioned(
      duration: const Duration(milliseconds: 350),
      curve: Curves.easeOutCubic,
      bottom: visible ? 0 : -260,
      left: 0,
      right: 0,
      child: TweenAnimationBuilder<double>(
        duration: const Duration(milliseconds: 450),
        tween: Tween(begin: visible ? 0.05 : 0, end: 0),
        curve: Curves.elasticOut, // bounce effect
        builder: (context, bounce, child) {
          return Transform.translate(
            offset: Offset(0, -20 * bounce),
            child: child,
          );
        },
        child: Container(
          padding: EdgeInsets.fromLTRB(
            24,
            24,
            24,
            90 + bottomSafe,
          ),
          decoration: BoxDecoration(color: background),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  TweenAnimationBuilder<double>(
                    duration: const Duration(milliseconds: 450),
                    tween: Tween(begin: 0, end: visible ? 1 : 0),
                    curve: Curves.easeOut,
                    builder: (context, value, child) {
                      double scale = 1.0;
                      double dx = 0;

                      if (isCorrect) {
                        // success pulse
                        scale = 1 +
                            (sin(value * pi) *
                                0.3); // pulse between 100% and 130%
                      } else {
                        // shake animation
                        dx = sin(value * 8 * pi) *
                            4; // shake left-right 4 times with 4px amplitude
                      }

                      return Transform.translate(
                        offset: Offset(dx, 0),
                        child: Transform.scale(
                          scale: scale,
                          child: child,
                        ),
                      );
                    },
                    child: Icon(
                      isCorrect ? Icons.check_circle : Icons.cancel,
                      color: accent,
                      size: 35,
                    ),
                  ),
                  const SizedBox(width: 10),
                  Text(
                    isCorrect ? "Correct!" : "Not quite",
                    style: TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: accent,
                    ),
                  ),
                ],
              ),
              if (explanation != null) ...[
                const SizedBox(height: 10),
                Text(
                  explanation!,
                  style: TextStyle(
                    fontSize: 15,
                    height: 1.4,
                    fontWeight: FontWeight.w600,
                    color: isCorrect
                        ? const Color(0xFF2E7D32)
                        : const Color(0xFFF83B3B),
                  ),
                ),
              ],
              if (hint != null) ...[
                const SizedBox(height: 4),
                Text(
                  "Hint: $hint",
                  style: const TextStyle(
                    fontSize: 14,
                    height: 1.4,
                    fontStyle: FontStyle.italic,
                    color: Color(0xFF616161),
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}
