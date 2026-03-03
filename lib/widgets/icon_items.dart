import 'package:flutter/material.dart';

class IconItems extends StatelessWidget {
  final String path;
  final bool isSelected;
  final VoidCallback onTap;

  const IconItems({
    super.key,
    required this.path,
    required this.onTap,
    required this.isSelected,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        borderRadius: BorderRadius.circular(7),
        splashColor: Colors.transparent,
        highlightColor: const Color(0xFFE0E0E0), // 👈 gray when holding
        onTap: onTap,
        child: Ink(
          padding: const EdgeInsets.all(5),
          decoration: BoxDecoration(
            border: Border.all(
              color: isSelected ? const Color(0xFFFF4646) : Colors.transparent,
              width: 2.5,
            ),
            color: isSelected ? const Color(0xFFFFEBEB) : Colors.transparent,
            borderRadius: BorderRadius.circular(7),
          ),
          child: SizedBox(
            height: 30,
            child: Image.asset(path),
          ),
        ),
      ),
    );
  }
}
