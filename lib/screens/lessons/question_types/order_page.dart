import 'package:flutter/material.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/lesson/lesson_button.dart';
import 'package:vitalact/widgets/lesson/lesson_step_scaffold.dart';
import 'package:vitalact/services/lesson_progress_service.dart';
import 'package:vitalact/widgets/sprite_animation.dart';
import 'package:vitalact/models/steps/order_question.dart';

class OrderPage extends StatefulWidget {
  final OrderQuestionStep step;
  final Function(bool) onAnswered;

  const OrderPage({
    super.key,
    required this.step,
    required this.onAnswered,
  });

  @override
  State<OrderPage> createState() => _OrderPageState();
}

class _OrderPageState extends State<OrderPage> {
  late List<int> currentOrder;

  bool answered = false;
  bool? isCorrect;

  @override
  void initState() {
    super.initState();
    currentOrder = List.generate(widget.step.choices.length, (i) => i);
  }

  void reorder(int oldIndex, int newIndex) {
    if (answered) return;

    setState(() {
      if (newIndex > oldIndex) newIndex--;

      final item = currentOrder.removeAt(oldIndex);
      currentOrder.insert(newIndex, item);
    });
  }

  bool checkCorrect() {
    for (int i = 0; i < currentOrder.length; i++) {
      if (currentOrder[i] != widget.step.correctOrder[i]) {
        return false;
      }
    }
    return true;
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
      onButtonPressed: () {
        if (!answered) {
          final correct = checkCorrect();

          setState(() {
            answered = true;
            isCorrect = correct;
          });

          LessonProgressService.recordAnswer(
            correct,
            correct ? 10 : 0,
          );
        } else {
          widget.onAnswered(isCorrect!);
        }
      },
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

                  /// Sprite
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

                  ReorderableListView.builder(
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    onReorder: reorder,
                    itemCount: currentOrder.length,
                    proxyDecorator: (child, index, animation) {
                      return child;
                    },
                    itemBuilder: (context, index) {
                      final choiceIndex = currentOrder[index];

                      return Container(
                          key: ValueKey(choiceIndex),
                          margin: const EdgeInsets.only(bottom: 12),
                          child: AnimatedContainer(
                            duration: const Duration(milliseconds: 250),
                            curve: Curves.easeInOut,
                            child: Container(
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: SizedBox(
                                width: double.infinity,
                                child: Row(
                                  children: [
                                    Expanded(
                                      child: LessonButton(
                                        type: LessonButtonType.option,
                                        text:
                                            "${index + 1}. ${step.choices[choiceIndex]}",
                                        selected: false,
                                        onPressed: null,
                                      ),
                                    ),
                                    ReorderableDragStartListener(
                                      index: index,
                                      child: const Padding(
                                        padding: EdgeInsets.only(left: 8),
                                        child: Icon(Icons.drag_handle,
                                            color: AppColors.border),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ));
                    },
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
