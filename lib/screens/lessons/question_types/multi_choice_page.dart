import 'package:flutter/material.dart';
import 'package:vitalact/l10n/app_localizations.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import 'package:vitalact/widgets/lesson/lesson_step_scaffold.dart';
import 'package:vitalact/models/steps/multi_choice_step.dart';
import 'package:vitalact/services/lesson_progress_service.dart';
import 'package:vitalact/widgets/lesson/lesson_step_asset.dart';

class MultiChoicePage extends StatefulWidget {
  final MultiChoiceStep step;
  final Function(bool) onAnswered;

  const MultiChoicePage({
    super.key,
    required this.step,
    required this.onAnswered,
  });

  @override
  State<MultiChoicePage> createState() => _MultiChoicePageState();
}

class _MultiChoicePageState extends State<MultiChoicePage> {
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
    final l10n = AppLocalizations.of(context)!;
    final step = widget.step;

    return LessonStepScaffold(
      answered: answered,
      isCorrect: isCorrect,
      explanation: isCorrect == true
          ? step.correctExplanation
          : step.incorrectExplanation,
      hint: isCorrect == false ? step.hint : null,
      buttonText: answered ? l10n.nextButton : l10n.answerButton,
      onButtonPressed: selectedIndex != null
          ? () {
              if (!answered) {
                setState(() {
                  answered = true;
                  isCorrect = selectedIndex == widget.step.correctIndex;
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
                  Center(
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(16),
                      child: Stack(
                        children: [
                          Image.asset(
                            'assets/icons/redRectangle.png',
                            width: 170,
                          ),
                          LessonStepAsset(
                            asset: step.spriteAsset,
                            columns: 50,
                            rows: 1,
                            totalFrames: 50,
                            fps: 25,
                            height: 172,
                            width: 172,
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 28),
                  Column(
                    children: List.generate(step.choices.length, (index) {
                      return Padding(
                        padding: EdgeInsets.only(
                          bottom: index != step.choices.length - 1 ? 12 : 0,
                        ),
                        child: LessonButton(
                          type: LessonButtonType.option,
                          text: step.choices[index],
                          selected: selectedIndex == index,
                          onPressed: answered ? null : () => select(index),
                        ),
                      );
                    }),
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
