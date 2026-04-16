# PulseGuard 🫀

PulseGuard is a real-time patient monitoring dashboard built with C#, WPF, and .NET 8. It simulates and visualizes critical patient vitals such as electrocardiogram (ECG) waveforms and blood oxygen saturation (SpO2) using the Model-View-ViewModel (MVVM) architectural pattern. 

## Features
- **Real-Time Data Visualization**: High-performance rendering of live ECG and SpO2 readings using LiveCharts2.
- **Asynchronous Data Simulation**: Background services generate realistic, non-blocking patient vitals via the Task Parallel Library (TPL).
- **Automated Alerting System**: Evaluates incoming data streams and triggers immediate visual warnings when critical safety thresholds are breached.
- **Responsive UI/UX**: A dark-themed, medical-grade interface designed for clarity, with non-blocking asynchronous UI updates.
- **MVVM Architecture**: Strict separation of concerns ensuring testability and modularity.

## Tech Stack
- **Framework:** .NET 8, WPF (Windows Presentation Foundation)
- **Architecture:** MVVM Design Pattern
- **Libraries:** LiveChartsCore.SkiaSharpView.Wpf (LiveCharts2)
- **Concurrency:** TPL (Tasks, async/await), Background Services

## Getting Started

1. Clone the repository.
2. Ensure you have the .NET 8 SDK installed.
3. Open `PulseGuard.csproj` in Visual Studio or your preferred IDE.
4. Build and run the project.

## Project Structure
- `Models/`: Data structures mapping to patient vitals.
- `ViewModels/`: Presentation logic, bridging data bounds to Views.
- `Views/`: UI elements (XAML files).
- `Services/`: Background emulation and vital generation services.
