class Validators {
  static String? email(String? value) {
    if (value == null || value.isEmpty) {
      return "Email cannot be empty";
    }

    if (!value.contains('@')) {
      return "Enter a valid email";
    }

    return null;
  }

  static String? password(String? value) {
    if (value == null || value.isEmpty) {
      return "Password cannot be empty";
    }

    if (value.length < 6) {
      return "Password must be at least 6 characters";
    }

    return null;
  }
}
