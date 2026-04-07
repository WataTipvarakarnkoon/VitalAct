import 'package:flutter/material.dart';
import 'package:vitalact/models/lesson_module.dart';
import 'package:vitalact/models/lesson_item.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/sprite_animation.dart';
import 'package:vitalact/screens/lessons/lesson_runner_page.dart';
import 'package:vitalact/services/lesson_loader.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Stack(
      children: [
        Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              colors: [
                AppColors.background,
                AppColors.background,
                AppColors.gradient
              ],
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
            ),
          ),
        ),
        Positioned(
          top: height * -0.4,
          child: Image.asset(
            'assets/images/Background.png',
            width: width,
          ),
        ),
        Positioned(
          top: height * -0.39,
          left: width * -0.3,
          height: height * 0.5,
          child: Image.asset('assets/images/Ellipse.png'),
        ),

        /// 🔥 CONTINUE BUTTON (multi-module)
        Positioned(
          top: height * 0.08,
          left: 0,
          right: 0,
          child: Center(
            child: AppButton(
              onPressed: () {},
              width: width * 0.9,
              height: 65,
              borderRadius: 15,
              padding: const EdgeInsets.symmetric(horizontal: 0),
              backgroundColor: AppColors.primary,
              borderColor: AppColors.primary,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    "CONTINUE",
                    style: TextStyle(
                      fontSize: 16,
                      color: AppColors.background.withValues(alpha: 0.7),
                    ),
                  ),
                  FutureBuilder<List<LessonModule>>(
                    future: LessonLoader.loadAllModules(),
                    builder: (context, snapshot) {
                      if (!snapshot.hasData) {
                        return const Text(
                          "Loading...",
                          style: TextStyle(color: AppColors.background),
                        );
                      }

                      final modules = snapshot.data!;
                      final allLessons =
                          modules.expand((m) => m.lessons).toList();

                      final currentLesson = allLessons.first;

                      return Text(
                        currentLesson.title,
                        style: const TextStyle(
                          fontSize: 17,
                          color: AppColors.background,
                        ),
                      );
                    },
                  ),
                ],
              ),
            ),
          ),
        ),

        Positioned.fill(
          top: height * 0.157,
          child: const Lesson(),
        ),
      ],
    );
  }
}

class Lesson extends StatefulWidget {
  const Lesson({super.key});

  @override
  State<Lesson> createState() => _LessonState();
}

class _LessonState extends State<Lesson> {
  int currentStep = 0;
  int? pressedIndex;

  late Future<List<LessonModule>> moduleFuture;
  List<LessonItem> allLessons = [];

  @override
  void initState() {
    super.initState();
    moduleFuture = LessonLoader.loadAllModules();
  }

  Future<void> openLesson(int index) async {
    if (index > currentStep) return;

    final result = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => LessonRunnerPage(
          title: allLessons[index].title,
          steps: allLessons[index].steps,
        ),
      ),
    );

    if (result == true && index == currentStep) {
      setState(() {
        currentStep++;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<LessonModule>>(
      future: moduleFuture,
      builder: (context, snapshot) {
        if (!snapshot.hasData) {
          return const Center(child: CircularProgressIndicator());
        }

        final modules = snapshot.data!;

        final items = buildMixedList(modules);

        // store flat lessons for navigation
        allLessons = modules.expand((m) => m.lessons).toList();

        return buildLessonList(items);
      },
    );
  }

  /// 🔥 MIX MODULE + LESSON
  List<LessonListItem> buildMixedList(List<LessonModule> modules) {
    final List<LessonListItem> items = [];

    for (final module in modules) {
      items.add(LessonListItem.module(module.moduleTitle));

      for (final lesson in module.lessons) {
        items.add(LessonListItem.lesson(lesson));
      }
    }

    return items;
  }

  Widget buildLessonList(List<LessonListItem> items) {
    return SingleChildScrollView(
      physics: const SlowScrollPhysics(),
      child: Column(
        children: [
          const SizedBox(height: 40),
          ListView.separated(
            padding: const EdgeInsets.only(top: 10),
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: items.length,
            separatorBuilder: (_, __) => const SizedBox(height: 30),
            itemBuilder: (context, index) {
              final item = items[index];

              /// 🟡 MODULE HEADER
              if (item.isModule) {
                return Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 30),
                  child: Text(
                    item.moduleTitle!.toUpperCase(),
                    style: const TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.w900,
                      color: AppColors.primary,
                    ),
                  ),
                );
              }

              /// 🔴 LESSON
              final lesson = item.lesson!;

              final lessonIndex =
                  items.where((e) => e.lesson != null).toList().indexOf(item);

              final active = lessonIndex <= currentStep;
              final isPressed = pressedIndex == lessonIndex;

              return buildLessonCard(
                lesson: lesson,
                index: lessonIndex,
                active: active,
                isPressed: isPressed,
              );
            },
          ),
          const SizedBox(height: 60),
        ],
      ),
    );
  }

  Widget buildLessonCard({
    required LessonItem lesson,
    required int index,
    required bool active,
    required bool isPressed,
  }) {
    final width = MediaQuery.of(context).size.width;

    return Center(
      child: FractionallySizedBox(
        widthFactor: 0.8,
        child: Material(
          color: Colors.transparent,
          child: InkWell(
            onTap: active ? () => openLesson(index) : null,
            onHighlightChanged: (pressed) {
              setState(() {
                pressedIndex = pressed ? index : null;
              });
            },
            splashColor: Colors.transparent,
            highlightColor: Colors.transparent,
            child: AnimatedScale(
              scale: isPressed ? 0.9 : 1,
              duration: const Duration(milliseconds: 100),
              child: SizedBox(
                width: width * .8,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Stack(
                      children: [
                        Image.asset(
                          active
                              ? 'assets/images/red_lesson_box.png'
                              : 'assets/images/gray_lesson_box.png',
                        ),
                        Container(
                          height: 105,
                          padding: const EdgeInsets.only(left: 20),
                          child: Stack(
                            children: [
                              Positioned(
                                left: 195,
                                child: SpriteSheet(
                                  asset: lesson.spriteAsset.isNotEmpty
                                      ? lesson.spriteAsset
                                      : 'assets/spritesheet/NVSA.png',
                                  columns: 50,
                                  rows: 1,
                                  totalFrames: 50,
                                  fps: 30,
                                  width: 102,
                                  height: 102,
                                ),
                              ),
                              if (active)
                                Align(
                                  alignment: Alignment.centerLeft,
                                  child: SizedBox(
                                    width: 200,
                                    child: Text(
                                      lesson.title,
                                      style: const TextStyle(
                                        fontWeight: FontWeight.w700,
                                        fontSize: 23,
                                        color: AppColors.background,
                                      ),
                                    ),
                                  ),
                                ),
                            ],
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    if (active)
                      LinearProgressIndicator(
                        value: index < currentStep ? 1 : 0,
                        minHeight: 10,
                        backgroundColor: Colors.white,
                        color: AppColors.primary,
                      ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// 🔥 HELPER CLASS
class LessonListItem {
  final String? moduleTitle;
  final LessonItem? lesson;

  LessonListItem.module(this.moduleTitle) : lesson = null;
  LessonListItem.lesson(this.lesson) : moduleTitle = null;

  bool get isModule => moduleTitle != null;
}

/// 🔥 SCROLL PHYSICS
class SlowScrollPhysics extends BouncingScrollPhysics {
  const SlowScrollPhysics({super.parent});

  @override
  double applyPhysicsToUserOffset(ScrollMetrics position, double offset) {
    return offset * 0.3;
  }
}
