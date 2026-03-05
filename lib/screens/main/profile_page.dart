import 'package:flutter/material.dart';
import 'package:vitalact/widgets/icon_items.dart';
import 'settings_page.dart';

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Container(
            width: double.infinity,
            height: 150,
            color: const Color(0xFFFF4646),
          ),
          Container(
            width: double.infinity,
            margin: const EdgeInsets.only(top: 80),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Container(
                  width: 125,
                  height: 125,
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.white, width: 4),
                    color: const Color.fromARGB(255, 155, 38, 38),
                    shape: BoxShape.circle,
                  ),
                ),
                const SizedBox(height: 30),
                const Text(
                  "Name Surname",
                  style: TextStyle(
                      fontSize: 30,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFFFF4646)),
                ),
                const SizedBox(height: 50),
                SizedBox(
                  width: width * .8,
                  child: const Row(
                    children: [
                      Text(
                        "Email:",
                        style: TextStyle(
                            fontSize: 17,
                            fontWeight: FontWeight.w500,
                            color: Color.fromARGB(255, 132, 132, 132)),
                        textAlign: TextAlign.start,
                      ),
                      Expanded(
                        child: Text(
                          "XX.XXX@icloud.com",
                          style: TextStyle(
                              fontSize: 17,
                              fontWeight: FontWeight.w500,
                              color: Color.fromARGB(255, 132, 132, 132)),
                          textAlign: TextAlign.end,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(
                  height: 10,
                ),
                SizedBox(
                  width: width * .8,
                  child: const Row(
                    children: [
                      Text(
                        "Password:",
                        style: TextStyle(
                            fontSize: 17,
                            fontWeight: FontWeight.w500,
                            color: Color.fromARGB(255, 132, 132, 132)),
                        textAlign: TextAlign.start,
                      ),
                      Expanded(
                        child: Text(
                          "XXXXXXXXXXXXX",
                          style: TextStyle(
                              fontSize: 17,
                              fontWeight: FontWeight.w500,
                              color: Color.fromARGB(255, 132, 132, 132)),
                          textAlign: TextAlign.end,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(
                  height: 10,
                ),
                Container(
                  width: width * .8,
                  height: 1.5,
                  color: const Color.fromARGB(255, 132, 132, 132),
                ),
                const SizedBox(
                  height: 10,
                ),
                SizedBox(
                  width: width * .8,
                  child: const Row(
                    children: [
                      Text(
                        "Dark Mode:",
                        style: TextStyle(
                            fontSize: 17,
                            fontWeight: FontWeight.w500,
                            color: Color.fromARGB(255, 132, 132, 132)),
                        textAlign: TextAlign.start,
                      ),
                      Expanded(
                        child: Text(
                          "XXXXXXXXXXXXX",
                          style: TextStyle(
                              fontSize: 17,
                              fontWeight: FontWeight.w500,
                              color: Color.fromARGB(255, 132, 132, 132)),
                          textAlign: TextAlign.end,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(
                  height: 30,
                ),
                Container(
                  height: 37,
                  width: 125,
                  decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(20),
                      color: const Color.fromARGB(255, 132, 132, 132)),
                  child: const Center(
                    child: Text(
                      'LOG OUT',
                      style: TextStyle(
                          fontSize: 17,
                          color: Colors.white,
                          fontWeight: FontWeight.w700),
                    ),
                  ),
                ),
              ],
            ),
          ),

          // SETTINGS ICON (top-right)
          Positioned(
            top: 10,
            right: 20,
            child: IconItems(
              path: 'assets/icons/setting.png',
              isSelected: false, // not tied to bottom nav
              onTap: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) => const SettingsPage(),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
