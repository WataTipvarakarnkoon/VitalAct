import 'package:flutter/material.dart';
import 'package:vitalact/l10n/app_localizations.dart';
import 'package:vitalact/models/lesson_module.dart';
import 'package:vitalact/models/lesson_item.dart';
import 'package:vitalact/theme/app_colors.dart';
import 'package:vitalact/widgets/app_button.dart';
import 'package:vitalact/widgets/step_asset.dart';
import 'package:vitalact/screens/lessons/lesson_runner_page.dart';
import 'package:vitalact/services/lesson_loader.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final _lessonKey = GlobalKey<_LessonState>();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
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
                AppColors.gradient,
              ],
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
            ),
          ),
        ),
        Positioned(
          top: height * -0.4,
          child: Image.asset('assets/images/Background.png', width: width),
        ),
        Positioned(
          top: height * -0.39,
          left: width * -0.3,
          height: height * 0.5,
          child: Image.asset('assets/images/Ellipse.png'),
        ),
        Positioned(
          top: height * 0.08,
          left: width * 0.05,
          child: AppButton(
            onPressed: () {
              final lessonState = _lessonKey.currentState;
              if (lessonState != null) {
                lessonState.openLesson(lessonState.currentStep);
              }
            },
            width: width * 0.9,
            height: 70,
            borderRadius: 15,
            padding: const EdgeInsets.only(left: 30),
            alignment: Alignment.centerLeft,
            backgroundColor: AppColors.primary,
            borderColor: AppColors.primary,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  l10n.continueLesson,
                  style: TextStyle(
                    fontSize: 16,
                    height: 1.0,
                    color: AppColors.background.withValues(alpha: 0.7),
                  ),
                ),
                const SizedBox(height: 4),
                FutureBuilder<List<LessonModule>>(
                  future: LessonLoader.loadAllModules(
                    Localizations.localeOf(context).languageCode,
                  ),
                  builder: (context, snapshot) {
                    if (!snapshot.hasData) {
                      return Text(
                        l10n.loading,
                        style: const TextStyle(
                          color: AppColors.background,
                          height: 1.0,
                        ),
                      );
                    }

                    final lessonState = _lessonKey.currentState;
                    final modules = snapshot.data!;
                    final allLessons =
                        modules.expand((m) => m.lessons).toList();
                    final step = lessonState?.currentStep ?? 0;
                    final currentLesson =
                        allLessons[step.clamp(0, allLessons.length - 1)];

                    return Text(
                      currentLesson.title,
                      style: const TextStyle(
                        fontSize: 21,
                        height: 1.0,
                        color: AppColors.background,
                      ),
                    );
                  },
                ),
              ],
            ),
          ),
        ),
        Positioned.fill(
          top: height * 0.157,
          child: Lesson(key: _lessonKey),
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
  String? _loadedLocale;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    final locale = Localizations.localeOf(context).languageCode;
    if (_loadedLocale != locale) {
      _loadedLocale = locale;
      moduleFuture = LessonLoader.loadAllModules(locale);
    }
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
        allLessons = modules.expand((m) => m.lessons).toList();

        return buildLessonList(items);
      },
    );
  }

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
          const SizedBox(height: 20),
          ListView.separated(
            padding: const EdgeInsets.only(top: 10),
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: items.length,
            separatorBuilder: (_, i) => SizedBox(
              height: items[i].isModule ? 6 : 30,
            ),
            itemBuilder: (context, index) {
              final item = items[index];

              if (item.isModule) {
                return Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 35),
                  child: Text(
                    item.moduleTitle!.toUpperCase(),
                    style: const TextStyle(
                      fontSize: 30,
                      fontWeight: FontWeight.w900,
                      color: AppColors.primary,
                    ),
                  ),
                );
              }

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
                              if (active)
                                Positioned(
                                  left: 195,
                                  child: StepAsset(
                                    asset: lesson.spriteAsset.isNotEmpty
                                        ? lesson.spriteAsset
                                        : 'assets/spritesheet/NVSA.png',
                                    width: 102,
                                    height: 102,
                                    fps: 30,
                                  ),
                                ),
                              if (active)
                                Align(
                                  alignment: Alignment.centerLeft,
                                  child: SizedBox(
                                    width: 180,
                                    child: Text(
                                      lesson.title,
                                      style: const TextStyle(
                                        fontWeight: FontWeight.w700,
                                        fontSize: 26,
                                        height: 1.1,
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
                      Container(
                        height: 23,
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(20),
                          border: Border.all(
                            color: AppColors.primary.withValues(alpha: 0.5),
                            width: 2,
                          ),
                          boxShadow: [
                            BoxShadow(
                              color: AppColors.primary.withValues(alpha: 0.35),
                              blurRadius: 6,
                              offset: const Offset(0, 3),
                            ),
                          ],
                        ),
                        child: Padding(
                          padding: const EdgeInsets.all(2),
                          child: ClipRRect(
                            borderRadius: BorderRadius.circular(18),
                            child: LayoutBuilder(
                              builder: (context, constraints) {
                                final fill = index < currentStep ? 1.0 : 0.0;
                                return Stack(
                                  children: [
                                    Container(
                                      width: constraints.maxWidth,
                                      color: Colors.white,
                                    ),
                                    Container(
                                      width: constraints.maxWidth * fill,
                                      color: AppColors.primary,
                                    ),
                                  ],
                                );
                              },
                            ),
                          ),
                        ),
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

class LessonListItem {
  final String? moduleTitle;
  final LessonItem? lesson;

  LessonListItem.module(this.moduleTitle) : lesson = null;
  LessonListItem.lesson(this.lesson) : moduleTitle = null;

  bool get isModule => moduleTitle != null;
}

class SlowScrollPhysics extends BouncingScrollPhysics {
  const SlowScrollPhysics({super.parent});

  @override
  double applyPhysicsToUserOffset(ScrollMetrics position, double offset) {
    return offset * 0.3;
  }
}
