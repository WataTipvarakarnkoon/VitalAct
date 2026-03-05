import 'package:flutter/material.dart';
import '../../models/lesson_step.dart';
import '../../models/steps/reading_step.dart';
import 'question_types/reading_page.dart';

class LessonRunnerPage extends StatefulWidget {
  final List<LessonStep> steps;

  const LessonRunnerPage({
    super.key,
    required this.steps,
  });

  @override
  State<LessonRunnerPage> createState() => _LessonRunnerPageState();
}

class _LessonRunnerPageState extends State<LessonRunnerPage> {
  int currentIndex = 0;

  double get progress => (currentIndex + 1) / widget.steps.length;

  void nextStep() {
    if (currentIndex < widget.steps.length - 1) {
      setState(() {
        currentIndex++;
      });
    } else {
      Navigator.pop(context, true); // lesson completed
    }
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
              backgroundColor: const Color(0xFFFF4646),
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
    final step = widget.steps[currentIndex];

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
        backgroundColor: Colors.white,
        appBar: AppBar(
          backgroundColor: Colors.white,
          leading: IconButton(
            icon: const Icon(Icons.arrow_back),
            onPressed: () async {
              final shouldLeave = await _confirmExit();
              if (shouldLeave && context.mounted) {
                Navigator.pop(context);
              }
            },
          ),
          title: Text(
            "Lesson ${currentIndex + 1}/${widget.steps.length}",
          ),
          centerTitle: true,
          bottom: const PreferredSize(
            preferredSize: Size.fromHeight(1),
            child: Divider(
              height: 1,
              thickness: 1,
            ),
          ),
        ),
        body: Column(
          children: [
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 20, horizontal: 20),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(6),
                child: LinearProgressIndicator(
                  value: progress,
                  minHeight: 12,
                  backgroundColor: const Color(0xFFBDBDBD),
                  color: const Color(0xFFFF4646),
                ),
              ),
            ),
            Expanded(
              child: _buildStep(step),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStep(LessonStep step) {
    if (step is ReadingStep) {
      return ReadingPage(
        step: step,
        onNext: nextStep,
      );
    }

    return const Center(
      child: Text("Unsupported step type"),
    );
  }
}
