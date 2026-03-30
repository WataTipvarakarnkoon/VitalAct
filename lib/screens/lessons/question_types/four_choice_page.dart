import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import 'package:vitalact/widgets/lesson/lesson_step_scaffold.dart';
import 'package:vitalact/models/steps/multi_choice_step.dart';
import 'package:vitalact/services/lesson_progress_service.dart';
import 'package:vitalact/widgets/sprite_animation.dart';

class FourChoicePage extends StatefulWidget {
  final MultiChoiceStep step;
  final Function(bool) onAnswered;

  const FourChoicePage({
    super.key,
    required this.step,
    required this.onAnswered,
  });

  @override
  State<FourChoicePage> createState() => _FourChoicePageState();
}

class _FourChoicePageState extends State<FourChoicePage> {
  bool? isCorrect;
  int? selectedIndex;
  bool answered = false;

  void select(int index) {
    if (answered) return;

    setState(() {
      selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    final step = widget.step;

    return LessonStepScaffold(
      answered: answered,
      isCorrect: isCorrect,
      explanation: isCorrect == true
          ? step.correctExplanation
          : step.incorrectExplanation,
      hint: isCorrect == false ? step.hint : null,
      buttonText: answered ? "NEXT" : "ANSWER",
      onButtonPressed: selectedIndex != null
          ? () {
              if (!answered) {
                setState(() {
                  answered = true;
                  isCorrect = selectedIndex == step.correctIndex;
                });

                LessonProgressService.recordAnswer(
                  isCorrect!,
                  isCorrect! ? 10 : 0,
                );
              } else {
                widget.onAnswered(isCorrect!);
              }
            }
          : null,
      child: Column(
        children: [
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    step.instructions,
                    style: const TextStyle(
                      fontSize: 26,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 10),
                  Text(
                    step.title,
                    style: const TextStyle(
                      fontSize: 18,
                      height: 1.5,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 24),

                  /// 🔴 Image + Sprite
                  Center(
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(16),
                      child: Stack(
                        children: [
                          Image.asset(
                            'assets/icons/redRectangle.png',
                            width: 170,
                          ),
                          SpriteSheet(
                            asset: step.spriteAsset,
                            columns: 50,
                            rows: 1,
                            totalFrames: 50,
                            fps: 25,
                            height: 172,
                            width: 172,
                          )
                        ],
                      ),
                    ),
                  ),

                  const SizedBox(height: 28),

                  /// ✅ 2x2 GRID
                  GridView.builder(
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    itemCount: step.choices.length,
                    gridDelegate:
                        const SliverGridDelegateWithFixedCrossAxisCount(
                      crossAxisCount: 2,
                      mainAxisSpacing: 12,
                      crossAxisSpacing: 12,
                      childAspectRatio: 2.8,
                    ),
                    itemBuilder: (context, index) {
                      return LessonButton(
                        type: LessonButtonType.option,
                        text: step.choices[index],
                        selected: selectedIndex == index,
                        onPressed: answered ? null : () => select(index),
                      );
                    },
                  ),

                  const SizedBox(height: 16),

                  Text(
                    step.disclaimer,
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontSize: 13,
                      color: AppColors.textPrimary.withValues(alpha: 0.7),
                      fontWeight: FontWeight.w500,
                    ),
                  ),

                  const SizedBox(height: 40),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
