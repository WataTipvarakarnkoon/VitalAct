import 'package:flutter/material.dart';
import 'home_page.dart';
import 'profile_page.dart';
import 'practice_page.dart';
import '../../widgets/icon_items.dart';

class IndexPage extends StatefulWidget {
  const IndexPage({super.key});

  @override
  State<IndexPage> createState() => _IndexPageState();
}

class _IndexPageState extends State<IndexPage> {
  int selectedIndex = 2;

  final PageController _controller = PageController(initialPage: 2);
  final List<Widget> pages = const [
    ProfilePage(),
    PracticePage(),
    HomePage(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: PageView(
        controller: _controller,
        onPageChanged: (index) {},
        children: pages,
      ),
      bottomNavigationBar: SafeArea(
        child: BottomBar(
          selectedIndex: selectedIndex,
          onTap: (index) {
            setState(() {
              selectedIndex = index;
            });

            _controller.animateToPage(
              index,
              duration: const Duration(milliseconds: 300),
              curve: Curves.easeInOut,
            );
          },
        ),
      ),
    );
  }
}

class BottomBar extends StatelessWidget {
  final int selectedIndex;
  final Function(int) onTap;

  const BottomBar({
    super.key,
    required this.selectedIndex,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;

    return Container(
      height: 65,
      padding: EdgeInsets.symmetric(horizontal: width * 0.09),
      decoration: const BoxDecoration(
        border: Border(
          top: BorderSide(color: Color(0xFFFF4646), width: 3.5),
        ),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          IconItems(
            path: 'assets/icons/profile.png',
            isSelected: selectedIndex == 0,
            onTap: () => onTap(0),
          ),
          IconItems(
            path: 'assets/icons/home.png',
            isSelected: selectedIndex == 2,
            onTap: () => onTap(2),
          ),
          IconItems(
            path: 'assets/icons/practice.png',
            isSelected: selectedIndex == 1,
            onTap: () => onTap(1),
          ),
        ],
      ),
    );
  }
}
