import 'package:flutter/material.dart';
import 'package:vitalact/models/steps/multi_choice_step.dart';
import 'package:vitalact/models/steps/order_question.dart';
import 'package:vitalact/models/steps/text_input_step.dart';
import 'package:vitalact/screens/lessons/question_types/four_choice_page.dart';
import 'package:vitalact/screens/lessons/question_types/multi_choice_page.dart';
import 'package:vitalact/screens/lessons/question_types/text_input_page.dart';
import 'package:vitalact/screens/lessons/question_types/order_page.dart';
import 'package:vitalact/models/lesson_step.dart';
import 'package:vitalact/models/steps/reading_step.dart';
import 'package:vitalact/screens/lessons/question_types/reading_page.dart';
import 'package:animations/animations.dart';
import 'package:vitalact/theme/app_colors.dart';

class LessonRunnerPage extends StatefulWidget {
  final String title;
  final List<LessonStep> steps;

  const LessonRunnerPage({
    super.key,
    required this.title,
    required this.steps,
  });

  @override
  State<LessonRunnerPage> createState() => _LessonRunnerPageState();
}

class _LessonRunnerPageState extends State<LessonRunnerPage> {
  late List<LessonStep> currentSteps;
  late int totalSteps;
  int currentIndex = 0;
  int completedSteps = 0;
  int rounds = 0;

  @override
  void initState() {
    super.initState();
    currentSteps = List.from(widget.steps);
    totalSteps = widget.steps.length;
  }

  double get progress => (completedSteps) / totalSteps;

  List<LessonStep> repeatQueue = [];
  //Step Checked

  void stepChecked(bool isCorrect) {
    final step = currentSteps[currentIndex];

    if (!isCorrect) {
      repeatQueue.add(step);
    }

    nextStep();
  }

  //Next Step
  void nextStep() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;

      final step = currentSteps[currentIndex];

      if (step is ReadingStep) {
        completedSteps++;
      }

      if (step is MultiChoiceStep || step is TextInputStep) {
        if (!repeatQueue.contains(step)) {
          completedSteps++;
        }
      }

      if (currentIndex < currentSteps.length - 1) {
        setState(() {
          currentIndex++;
        });
      } else if (repeatQueue.isNotEmpty) {
        setState(() {
          currentSteps = List.from(repeatQueue);
          repeatQueue.clear();
          currentIndex = 0;
          rounds++;
        });
      } else {
        Navigator.pop(context, true);
      }
    });
  }

  Future<bool> _confirmExit() async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Leave lesson?"),
        content: const Text(
          "Your progress in this lesson will not be saved.",
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text("Stay"),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.primary,
            ),
            onPressed: () => Navigator.pop(context, true),
            child: const Text("Leave"),
          ),
        ],
      ),
    );

    return result ?? false;
  }

  @override
  Widget build(BuildContext context) {
    final step = currentSteps[currentIndex];

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) async {
        if (didPop) return;

        final shouldLeave = await _confirmExit();
        if (shouldLeave && context.mounted) {
          Navigator.pop(context);
        }
      },
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          backgroundColor: AppColors.background,
          surfaceTintColor: AppColors.background,
          leading: IconButton(
            padding: const EdgeInsets.only(bottom: 2),
            icon: const Icon(Icons.clear_rounded, color: AppColors.disabled),
            onPressed: () async {
              final shouldLeave = await _confirmExit();
              if (shouldLeave && context.mounted) {
                Navigator.pop(context);
              }
            },
          ),
          title: Text(
            widget.title,
            style: const TextStyle(
              fontSize: 20,
              color: AppColors.disabled,
              fontWeight: FontWeight.w500,
            ),
          ),
          centerTitle: true,
          bottom: const PreferredSize(
            preferredSize: Size.fromHeight(1),
            child: Divider(
              color: AppColors.disabled,
              height: 1,
              thickness: 1,
            ),
          ),
        ),
        body: Column(
          children: [
            // PROGRESS BAR
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 20, horizontal: 20),
              child: TweenAnimationBuilder<double>(
                duration: const Duration(milliseconds: 400),
                curve: Curves.easeInOut,
                tween: Tween<double>(
                  begin: 0,
                  end: progress,
                ),
                builder: (context, value, _) {
                  return LinearProgressIndicator(
                    value: value,
                    minHeight: 12,
                    backgroundColor: AppColors.disabled,
                    color: AppColors.primary,
                    borderRadius: BorderRadius.circular(6),
                  );
                },
              ),
            ),

            Expanded(
              child: PageTransitionSwitcher(
                duration: const Duration(milliseconds: 400),
                transitionBuilder:
                    (child, primaryAnimation, secondaryAnimation) {
                  final curvedPrimary = CurvedAnimation(
                    parent: primaryAnimation,
                    curve: Curves.easeInOut,
                  );

                  final curvedSecondary = CurvedAnimation(
                    parent: secondaryAnimation,
                    curve: Curves.easeInOut,
                  );

                  return SlideTransition(
                    position: Tween<Offset>(
                      begin: const Offset(1, 0),
                      end: Offset.zero,
                    ).animate(curvedPrimary),
                    child: SlideTransition(
                      position: Tween<Offset>(
                        begin: Offset.zero,
                        end: const Offset(-1, 0),
                      ).animate(curvedSecondary),
                      child: child,
                    ),
                  );
                },
                child: Container(
                  key: ValueKey('$rounds-$currentIndex'),
                  child: _buildStep(step),
                ),
              ),
            )
          ],
        ),
      ),
    );
  }

//next step thing
  Widget _buildStep(LessonStep step) {
    if (step is ReadingStep) {
      return ReadingPage(
        step: step,
        onNext: nextStep,
      );
    } else if (step is MultiChoiceStep) {
      if (step.choices.length == 4) {
        return FourChoicePage(
          step: step,
          onAnswered: stepChecked,
        );
      }
      return MultiChoicePage(
        step: step,
        onAnswered: stepChecked,
      );
    } else if (step is TextInputStep) {
      return TextInputPage(
        step: step,
        onAnswered: stepChecked,
      );
    } else if (step is OrderQuestionStep) {
      return OrderPage(
        step: step,
        onAnswered: stepChecked,
      );
    }

    return const Center(
      child: Text("Unsupported step type"),
    );
  }
}
