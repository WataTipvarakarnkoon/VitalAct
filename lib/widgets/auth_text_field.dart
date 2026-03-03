import 'package:flutter/material.dart';

class AuthTextField extends StatefulWidget {
  final String hintText;
  final bool isPassword;
  final TextEditingController controller;
  final String? Function(String?)? validator;
  final Iterable<String>? autofillHints;

  const AuthTextField({
    super.key,
    required this.hintText,
    required this.controller,
    this.validator,
    this.isPassword = false,
    this.autofillHints,
  });

  @override
  State<AuthTextField> createState() => _AuthTextFieldState();
}

class _AuthTextFieldState extends State<AuthTextField> {
  bool _obscureText = true;
  String? _errorText;

  @override
  void initState() {
    super.initState();
    _obscureText = widget.isPassword;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        TextFormField(
          style: const TextStyle(
              fontWeight: FontWeight.w700,
              fontSize: 17,
              color: Color.fromARGB(255, 119, 119, 119)),
          controller: widget.controller,
          obscureText: widget.isPassword ? _obscureText : false,
          autovalidateMode: AutovalidateMode.onUserInteraction,
          validator: widget.validator,
          autofillHints: widget.autofillHints,
          decoration: InputDecoration(
            hintStyle: const TextStyle(
                fontWeight: FontWeight.w700,
                fontSize: 17,
                color: Color.fromARGB(255, 119, 119, 119)),
            errorText: _errorText,
            hintText: widget.hintText,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 20,
              vertical: 14,
            ),
            suffixIcon: widget.isPassword
                ? IconButton(
                    icon: Icon(
                      _obscureText ? Icons.visibility_off : Icons.visibility,
                    ),
                    onPressed: () {
                      setState(() {
                        _obscureText = !_obscureText;
                      });
                    },
                  )
                : null,
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(50),
              borderSide: const BorderSide(
                color: Color(0xFFC62828),
                width: 2,
              ),
            ),
            focusedErrorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(50),
              borderSide: const BorderSide(
                color: Color(0xFFC62828),
                width: 2,
              ),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(50),
              borderSide: const BorderSide(
                color: Color(0xFFC4C4C4),
                width: 2,
              ),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(50),
              borderSide: const BorderSide(
                color: Color(0xFF00AAFF),
                width: 2,
              ),
            ),
          ),
        ),
      ],
    );
  }
}
