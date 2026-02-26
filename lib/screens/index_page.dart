import 'package:flutter/material.dart';
import 'package:vitalact/main.dart';

class IndexPage extends StatelessWidget {
  const IndexPage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final height = MediaQuery.of(context).size.height;

    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Positioned(
            top: height * -.4,
            width: width,
            child: Image.asset('assets/images/Background.png'),
          ),

          Positioned(
            top: height * -.39,
            left: width * -.3,
            height: height * 0.5,
            child: Image.asset('assets/images/Ellipse.png'),
          ),
        ],
      ),

      bottomNavigationBar: SafeArea(child: const BottomBar()),
    );
  }
}

class BottomBar extends StatefulWidget {
  const BottomBar({super.key});

  @override
  State<BottomBar> createState() => _BottomBarState();
}

class _BottomBarState extends State<BottomBar> {
  int selected = 2;

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    return Container(
      height: 60,
      padding: EdgeInsets.symmetric(horizontal: width * 0.09),
      decoration: BoxDecoration(
        border: Border(top: BorderSide(color: Color(0xFFFF4646), width: 4)),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          IconItems(
            path: 'assets/icons/profile.png',
            isSelected: selected == 0,
            onTap: () {
              setState(() => selected = 0);
            },
          ),
          IconItems(
            path: 'assets/icons/practice.png',
            isSelected: selected == 1,
            onTap: () {
              setState(() => selected = 1);
            },
          ),
          IconItems(
            path: 'assets/icons/home.png',
            isSelected: selected == 2,
            onTap: () {
              setState(() => selected = 2);
            },
          ),
          IconItems(
            path: 'assets/icons/setting.png',
            isSelected: selected == 3,
            onTap: () {
              setState(() => selected = 3);
            },
          ),
        ],
      ),
    );
  }
}
