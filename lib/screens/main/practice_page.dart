import 'package:vitalact/screens/main/test_unity.dart';
import 'package:flutter/material.dart';

class PracticePage extends StatefulWidget {
  const PracticePage({super.key});

  @override
  State<PracticePage> createState() => _PracticePageState();
}

class _PracticePageState extends State<PracticePage>
    with SingleTickerProviderStateMixin {
  bool isPressed = false;
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return AnimatedBuilder(
      animation: _tabController.animation!,
      builder: (context, _) {
        final double t = _tabController.animation!.value;
        final Color themeColor = Color.lerp(Colors.red, Colors.blue, t)!;

        return AnimatedContainer(
          duration: const Duration(milliseconds: 150),
          color: themeColor.withValues(alpha: 0.05),
          child: Column(
            children: [
              const SizedBox(height: 40),
              TabBar(
                indicatorWeight: 1,
                indicatorSize: TabBarIndicatorSize.tab,
                controller: _tabController,
                indicatorColor: themeColor,
                labelColor: themeColor,
                unselectedLabelColor: Colors.grey,
                labelStyle:
                    const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                unselectedLabelStyle:
                    const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                tabs: const [
                  Tab(text: "Mental Drill"),
                  Tab(text: "Physical Drill"),
                ],
              ),
              Expanded(
                child: TabBarView(
                  controller: _tabController,
                  children: [
                    Center(
                      child: Column(
                        children: [
                          const SizedBox(
                            height: 25,
                          ),
                          Text(
                            "Practice",
                            style: TextStyle(
                              fontSize: 25,
                              fontWeight: FontWeight.w800,
                              color: themeColor,
                            ),
                          ),
                          const Text('Sharpen your emergency skills',
                              style: TextStyle(
                                fontSize: 17,
                                fontWeight: FontWeight.w500,
                                color: Color.fromARGB(255, 116, 116, 116),
                              )),
                        ],
                      ),
                    ),
                    Center(
                      child: Column(
                        children: [
                          const SizedBox(
                            height: 25,
                          ),
                          Text(
                            "Practice",
                            style: TextStyle(
                              fontSize: 25,
                              fontWeight: FontWeight.w800,
                              color: themeColor,
                            ),
                          ),
                          const Text('Sharpen your emergency skills',
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.w500,
                                color: Color.fromARGB(255, 116, 116, 116),
                              )),
                          const SizedBox(
                            height: 40,
                          ),
                          GestureDetector(
                            onTapDown: (_) => setState(() => isPressed = true),
                            onTapUp: (_) => setState(() => isPressed = false),
                            onTapCancel: () =>
                                setState(() => isPressed = false),
                            onTap: () {
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => TestUnity(),
                                ),
                              );
                            },
                            child: AnimatedScale(
                              scale: isPressed ? 0.8 : 0.9,
                              duration: const Duration(milliseconds: 100),
                              child: Image.asset(
                                  'assets/images/Emergency Simulator.png'),
                            ),
                          )
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}
