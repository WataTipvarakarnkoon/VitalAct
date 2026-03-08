import 'package:flutter/material.dart';

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
    final Color background =
        isCorrect ? const Color(0xFFDFF6DD) : const Color(0xFFFFE5E5);

    final Color accent =
        isCorrect ? const Color(0xFF2E7D32) : const Color(0xFFF83B3B);

    return Align(
      alignment: Alignment.bottomCenter,
      child: AnimatedSlide(
        duration: const Duration(milliseconds: 350),
        curve: Curves.easeOutCubic,
        offset: visible ? Offset.zero : const Offset(0, 2),
        child: Container(
          width: double.infinity,
          padding: const EdgeInsets.fromLTRB(
            24,
            24,
            24,
            90,
          ),
          decoration: BoxDecoration(color: background),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(
                    isCorrect ? Icons.check_circle : Icons.cancel,
                    color: accent,
                    size: 35,
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
                  "Hint: ${hint!}",
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
