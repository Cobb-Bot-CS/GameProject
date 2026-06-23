import 'package:flutter/material.dart';

void main() {
  runApp(CalculatorApp());
}

class CalculatorApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Simple Calculator',
      theme: ThemeData(
        primarySwatch: Colors.deepPurple,
      ),
      home: CalculatorHome(),
      debugShowCheckedModeBanner: false,
    );
  }
}

class CalculatorHome extends StatefulWidget {
  @override
  _CalculatorHomeState createState() => _CalculatorHomeState();
}

class _CalculatorHomeState extends State<CalculatorHome> {
  final TextEditingController num1Controller = TextEditingController();
  final TextEditingController num2Controller = TextEditingController();
  String result = "";

  void calculate(String operator) {
    final double? num1 = double.tryParse(num1Controller.text);
    final double? num2 = double.tryParse(num2Controller.text);

    if (num1 == null || num2 == null) {
      setState(() {
        result = "Enter valid numbers!";
      });
      return;
    }

    double res;
    switch (operator) {
      case '+':
        res = num1 + num2;
        break;
      case '-':
        res = num1 - num2;
        break;
      case '*':
        res = num1 * num2;
        break;
      case '/':
        if (num2 == 0) {
          result = "Cannot divide by zero";
          return;
        }
        res = num1 / num2;
        break;
      default:
        result = "Invalid Operation";
        return;
    }

    setState(() {
      result = "Result: ${res.toStringAsFixed(2)}";
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Simple Calculator'),
      ),
      body: Padding(
        padding: EdgeInsets.all(20.0),
        child: Column(
          children: [
            TextField(
              controller: num1Controller,
              decoration: InputDecoration(labelText: 'Enter first number'),
              keyboardType: TextInputType.number,
            ),
            TextField(
              controller: num2Controller,
              decoration: InputDecoration(labelText: 'Enter second number'),
              keyboardType: TextInputType.number,
            ),
            SizedBox(height: 20),
            Wrap(
              spacing: 10,
              children: [
                ElevatedButton(
                    onPressed: () => calculate('+'), child: Text('+')),
                ElevatedButton(
                    onPressed: () => calculate('-'), child: Text('-')),
                ElevatedButton(
                    onPressed: () => calculate('*'), child: Text('*')),
                ElevatedButton(
                    onPressed: () => calculate('/'), child: Text('/')),
              ],
            ),
            SizedBox(height: 20),
            Text(
              result,
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            )
          ],
        ),
      ),
    );
  }
}
