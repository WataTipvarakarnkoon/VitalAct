import 'package:flutter/material.dart';

class IconItems extends StatelessWidget {
  final String path;
  final String selectedPath;
  final bool isSelected;
  final VoidCallback onTap;

  const IconItems(
      {super.key,
      required this.path,
      required this.onTap,
      required this.isSelected,
      required this.selectedPath});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        borderRadius: BorderRadius.circular(7),
        splashColor: Colors.transparent,
        highlightColor: const Color(0xFFE0E0E0),
        onTap: onTap,
        child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 35),
            height: 30,
            child: AnimatedSwitcher(
              duration: const Duration(milliseconds: 250),
              child: Image.asset(
                isSelected ? path : selectedPath,
                key: ValueKey(isSelected),
              ),
            )),
      ),
    );
  }
}
