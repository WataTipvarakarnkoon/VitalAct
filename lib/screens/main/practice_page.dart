import 'package:vitalact/screens/main/test_unity.dart';
import 'package:flutter/material.dart';
import 'test_unity.dart';

class PracticePage extends StatefulWidget {
  const PracticePage({super.key});

  @override
  State<PracticePage> createState() => _PracticePageState();
}

class _PracticePageState extends State<PracticePage>
    with SingleTickerProviderStateMixin {
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
    return AnimatedBuilder(
      animation: _tabController.animation!,
      builder: (context, _) {
        final double t = _tabController.animation!.value;
        final Color themeColor = Color.lerp(Colors.red, Colors.blue, t)!;

        return AnimatedContainer(
          duration: const Duration(milliseconds: 150),
          color: themeColor.withOpacity(0.05),
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
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(
                            "Mental Content",
                            style: TextStyle(
                              fontSize: 22,
                              fontWeight: FontWeight.bold,
                              color: themeColor,
                            ),
                          ),
                          const SizedBox(height: 12),
                          ElevatedButton(
                            onPressed: () {
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => TestUnity(),
                                ),
                              );
                            },
                            child: const Text("Open Unity Test"),
                          ),
                        ],
                      ),
                    ),
                    Center(
                      child: Text(
                        "Physical Content",
                        style: TextStyle(
                          fontSize: 22,
                          fontWeight: FontWeight.bold,
                          color: themeColor,
                        ),
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
