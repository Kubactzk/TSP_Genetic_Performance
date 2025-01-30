import matplotlib.pyplot as plt
from scipy.optimize import curve_fit
import numpy as np
from scipy.stats import norm


def read_data_from_file_connections(file_name):
    with open(file_name, 'r') as file:
        lines = file.readlines()

    # Parse the data
    size = int(lines[0].strip())  # First line: size

    # Second line: connections as a list of integers
    connections = list(map(int, lines[1].strip().split(', ')))

    # Third and onwards: points as tuples
    points = []
    for line in lines[2:]:
        try:
            points.append(tuple(map(int, line.strip().strip('()').split(', '))))
        except ValueError:
            print(f"Warning: Could not parse line: {line.strip()}")

    return size, connections, points


def generate_plot(size, connections, points):
    # Extract X and Y coordinates from points
    x_points = [point[0] for point in points]
    y_points = [point[1] for point in points]

    # Create a scatter plot for points
    plt.figure(figsize=(10, 6))
    plt.scatter(x_points, y_points, color='blue', label='Points')

    # Draw the connections (edges) between the points
    for i in range(len(connections) - 1):
        start = connections[i]  # Starting index
        end = connections[i + 1]  # Ending index
        if start < len(points) and end < len(points):
            plt.plot([x_points[start], x_points[end]], [y_points[start], y_points[end]], 'r-', lw=2)

    # Highlight the start and end points
    if connections[0] < len(points):
        plt.scatter(x_points[connections[0]], y_points[connections[0]], color='green', s=100, label='Start')
    if connections[-1] < len(points):
        plt.scatter(x_points[connections[-1]], y_points[connections[-1]], color='red', s=100, label='End')

    # Set labels and title
    plt.title(f"Visualization of Size {size}")
    plt.xlabel("X Coordinates")
    plt.ylabel("Y Coordinates")
    plt.legend()
    plt.grid(True)
    plt.show()


def plotFormulaData(filename):
    # Read data
    sizes, max_fitness, min_fitness = read_data_from_file(filename)

    # Create the plot
    plt.figure(figsize=(12, 6))
    plt.plot(sizes, max_fitness, label='Average Max Fitness', marker='o', linestyle='-', color='blue')
    plt.plot(sizes, min_fitness, label='Average Min Fitness', marker='o', linestyle='-', color='red')

    # Add grid
    plt.grid(visible=True, linestyle='--', alpha=0.6)

    # Add labels and title
    plt.xlabel('Number of Points')
    plt.ylabel('Fitness Value')
    plt.title('Fitness vs. Number of Points')

    # Add legend
    plt.legend(loc='upper left')

    # Add data points with slight offset for clarity
    for x, y_max, y_min in zip(sizes, max_fitness, min_fitness):
        plt.text(x, y_max, f'{y_max:.2f}', fontsize=10, ha='center', color='black', verticalalignment='bottom')
        plt.text(x, y_min, f'{y_min:.2f}', fontsize=10, ha='center', color='black', verticalalignment='top')

    # Show the plot
    plt.tight_layout()
    plt.savefig("population_formula/camparing_upgraded.png")
    plt.close()

# Define the power law model for curve fitting
def power_law(x, a, b):
    return a * x ** b

# Function to fit the population data and return the parameters
def fit_population_data(sizes, ratios):
    # Fit the data to the power law model
    params, _ = curve_fit(power_law, sizes, ratios)
    a, b = params
    return a, b

# Function to calculate population size for a given target ratio
def find_population_size(target_ratio, a, b):
    # Solve for Size from the power law model: Size = (Ratio / a)^(1 / b)
    size = (target_ratio / a) ** (1 / b)
    return size

# Function to read the data from file
def read_data_from_file(filename):
    sizes = []
    max_fitness = []
    min_fitness = []

    with open(filename, 'r') as file:
        lines = file.readlines()

        # Loop through the lines in steps of 3 (size, max fitness, min fitness)
        for i in range(0, len(lines), 3):
            try:
                # Extract the size (first line)
                size = int(lines[i].strip())
                sizes.append(size)

                # Extract the max fitness (second line)
                max_fitness_value = float(lines[i+1].replace(',', '.').strip())
                max_fitness.append(max_fitness_value)

                # Extract the min fitness (third line)
                min_fitness_value = float(lines[i+2].replace(',', '.').strip())
                min_fitness.append(min_fitness_value)
            except (ValueError, IndexError) as e:
                print(f"Warning: Could not parse a line set at index {i}: {e}")
    
    return sizes, max_fitness, min_fitness

# Function to calculate and plot the result for the target ratio
def plot_population_size_for_target_ratio(filename, target_ratio=2.3):
    # Read data
    sizes, max_fitness, min_fitness = read_data_from_file(filename)

    # Calculate the ratio of max fitness to min fitness
    max_min_ratio = [max_val / min_val if min_val != 0 else float('inf') for max_val, min_val in zip(max_fitness, min_fitness)]
    
    # Fit the population data
    a, b = fit_population_data(sizes, max_min_ratio)

    # Find the population size for the target ratio
    required_size = find_population_size(target_ratio, a, b)

    # Print the calculated population size
    print(f"The estimated population size for a target ratio of {target_ratio} is: {required_size:.2f}")

    # Create the plot with the fitted curve and the target ratio
    plt.figure(figsize=(12, 6))

    # Plot the original data
    plt.plot(sizes, max_min_ratio, label='Max/Min Ratio', marker='o', linestyle='-', color='green')

    # Plot the fitted curve
    fitted_curve = power_law(np.array(sizes), a, b)
    plt.plot(sizes, fitted_curve, label=f'Fitted Power Law: $y = {a:.4f} x^{{{b:.4f}}}$', linestyle='--', color='red')

    # Mark the target ratio
    plt.axhline(target_ratio, color='blue', linestyle='--', label=f'Target Ratio = {target_ratio}')

    # Add grid
    plt.grid(visible=True, linestyle='--', alpha=0.6)

    # Add labels and title
    plt.xlabel('Number of Points')
    plt.ylabel('Max/Min Fitness Ratio')
    plt.title(f'Max/Min Fitness Ratio vs. Number of Points (Target Ratio = {target_ratio})')

    # Add legend
    plt.legend(loc='upper left')

    # Show the plot
    plt.tight_layout()
    plt.savefig("population_formula/max_min_ratio_with_target.png")
    plt.close()

def plotFormulaDataRatio(filename):
    # Read data
    sizes, max_fitness, min_fitness = read_data_from_file(filename)

    # Calculate the ratio of max fitness to min fitness
    max_min_ratio = [max_val / min_val if min_val != 0 else float('inf') for max_val, min_val in zip(max_fitness, min_fitness)]

    # Create the plot
    plt.figure(figsize=(12, 6))

    # Plot the max/min ratio
    plt.plot(sizes, max_min_ratio, label='Max/Min Ratio', marker='o', linestyle='-', color='green')

    # Add grid
    plt.grid(visible=True, linestyle='--', alpha=0.6)

    # Add labels and title
    plt.xlabel('Number of Points')
    plt.ylabel('Max/Min Fitness Ratio')
    plt.title('Max/Min Fitness Ratio vs. Number of Points')

    # Add legend
    plt.legend(loc='upper left')

    # Add data points with slight offset for clarity
    for x, ratio in zip(sizes, max_min_ratio):
        plt.text(x, ratio, f'{ratio:.2f}', fontsize=10, ha='center', color='black', verticalalignment='bottom')

    # Show the plot
    plt.tight_layout()
    plt.savefig("population_formula/max_min_ratio_fixed.png")
    plt.close()

def lined(path):
    sizes, max_vals, avg_vals, min_vals = [], [], [], []

    with open(path, "r") as file:
        for line in file:
            parts = line.strip().split()
            if len(parts) == 4:
                size = int(parts[0])
                max_val = float(parts[1].replace(",", "."))
                avg_val = float(parts[2].replace(",", "."))
                min_val = float(parts[3].replace(",", "."))

                sizes.append(size)
                max_vals.append(max_val)
                avg_vals.append(avg_val)
                min_vals.append(min_val)
    return sizes, max_vals, avg_vals, min_vals

def plotLined(sizes, max_vals, avg_vals, min_vals,title, path):
    # Plot the data
    plt.figure(figsize=(10, 6))
    plt.plot(sizes, max_vals, marker="o", linestyle="-", label="Max")
    plt.plot(sizes, avg_vals, marker="s", linestyle="--", label="Average")
    plt.plot(sizes, min_vals, marker="d", linestyle="-.", label="Min")

    # Add data point labels
    for x, y in zip(sizes, max_vals):
        plt.text(x, y, f"{y:.2f}", ha="right", va="bottom", fontsize=9)
    for x, y in zip(sizes, avg_vals):
        plt.text(x, y, f"{y:.2f}", ha="right", va="bottom", fontsize=9)
    for x, y in zip(sizes, min_vals):
        plt.text(x, y, f"{y:.2f}", ha="right", va="bottom", fontsize=9)

    # Labels and formatting
    plt.xlabel("Size")
    plt.ylabel("Time [ms]")
    plt.title(title)
    plt.xticks(sizes)  # Ensure x-axis has the exact data points
    plt.grid(True, linestyle="--", alpha=0.6)
    plt.legend()
    plt.savefig(path)
    plt.close()

def plot_speedup(sizes, speedup_list, labels, title, path):
    plt.figure(figsize=(10, 6))
    
    # Define colors and markers for each curve
    colors = ['b', 'g', 'r', 'c']
    markers = ['o', 's', '^', 'D']
    
    # Plot each speedup curve
    for i, (speedup, label) in enumerate(zip(speedup_list, labels)):
        plt.plot(sizes, speedup, marker=markers[i], linestyle='-', color=colors[i], label=label)
        
        # Annotate data points
        for j, txt in enumerate(speedup):
            plt.annotate(f'{txt:.1f}%', (sizes[j], speedup[j]), textcoords="offset points", xytext=(0, 5), ha='center')
    
    # Set x and y axes labels
    plt.xlabel('Problem Size', fontsize=12)
    plt.ylabel('Speedup (%)', fontsize=12)
    
    # Use log scale for both axes
    plt.yscale('log')
    plt.xscale('log')
    
    # Add grid
    plt.grid(True, which="both", ls="--", linewidth=0.5)
    
    # Set the title
    plt.title(title, fontsize=14)
    
    # Show the plot with a legend
    plt.legend(loc='best')
    plt.tight_layout()
    plt.savefig(path)
    plt.close()
    

def read_benchmark_data(file_path):
    # Initialize lists to store the data
    sizes = []
    benchmarks = []

    # Open the file and read the data
    with open(file_path, 'r') as file:
        for line in file:
            # Split the line into components
            parts = line.strip().split()
            
            # The first part is the size
            size = int(parts[0])
            sizes.append(size)
            
            # The rest are benchmark values
            benchmark_values = [float(value.replace(',', '.')) for value in parts[1:]]
            benchmarks.append(benchmark_values)
    
    return sizes, benchmarks

def plot_histograms(sizes, benchmarks, e):
    # Loop through each size and its corresponding benchmark values
    for size, benchmark_values in zip(sizes, benchmarks):
        # Create a histogram for the current size
        plt.figure()  # Create a new figure for each histogram
        plt.hist(benchmark_values, bins=20, edgecolor='black', alpha=0.7, density=True, label='Histogram')
        
        # Add grid
        plt.grid(axis='both', linestyle='--', alpha=0.7)
        
        # Calculate the normal distribution parameters (mean and standard deviation)
        mean = np.mean(benchmark_values)
        std = np.std(benchmark_values)
        
        # Generate a range of values for the normal distribution line
        x = np.linspace(min(benchmark_values), max(benchmark_values), 1000)
        y = norm.pdf(x, mean, std)  # Probability density function
        
        # Plot the normal distribution line
        plt.plot(x, y, 'r-', linewidth=2, label='Normal Distribution')
        
        # Customize the plot
        plt.title(f'Histogram of Benchmark Values for Size {size}')
        plt.xlabel('Benchmark Values')
        plt.ylabel('Density')
        plt.legend()
        plt.savefig(e+f"{size}_histogram_parallel.png")

if __name__ == "__main__":
    seq = "benchmark_data.txt"
    parallel = "benchmark_data_parallel.txt"
    path = r"D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\lined\\double\\"
    pathLong = r"D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\lined\\long\\"
    pathLinedInt = path+seq
    pathLinedIntParallel = path+parallel

    sizes, max_vals, avg_vals, min_vals = lined(path+seq)
    sizes2, max_vals2, avg_vals2, min_vals2 = lined(path+"benchmark_data_parallel_2_cores.txt")
    sizes3, max_vals3, avg_vals3, min_vals3 = lined(path+"benchmark_data_parallel_3_cores.txt")
    sizes4, max_vals4, avg_vals4, min_vals4 = lined(path+parallel)

    # sizes, benchmarks = read_benchmark_data(path+"benchmark_data_parallel_hist.txt")
    # plot_histograms(sizes, benchmarks, path)
    speedup2 = []
    speedup3 = []
    speedup4 = []
    eff2 = []
    eff3 = []
    eff4 = []
    for i in range(len(sizes)):
        speedup2.append((avg_vals[i]/avg_vals2[i]))
        eff2.append((avg_vals[i]/avg_vals2[i])/4)

        speedup3.append((avg_vals[i]/avg_vals3[i]))
        eff3.append((avg_vals[i]/avg_vals3[i])/4)

        speedup4.append((avg_vals[i]/avg_vals4[i]))
        eff4.append((avg_vals[i]/avg_vals4[i])/4)

    speedup_list = [speedup2, speedup3, speedup4]
    labels = ['2 Cores', '3 Cores', '4 Cores']
    title = 'Speedup Comparison for Different Cores'
    plot_speedup(sizes, speedup_list, labels, title, path+"speedupCombined.png")
    #plot_speedup(sizes, speedup_list, "Speedup for double", path+"speedupLog.png"
    # plotLined(sizes, max_vals, avg_vals, min_vals, "Double sequentual benchmark", path+"speedupCombined.png")
    # plotLined(sizes, max_valsParallel, avg_valsParallel, min_valsParallel,"Double parallel benchmark", path+"parallel.png")


    # plotFormulaData(r"D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\population_formula\\population_size_formula_data.txt")
    # plotFormulaDataRatio(r"D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\population_formula\\population_size_formula_data.txt")
    # plot_population_size_for_target_ratio(r"D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\population_formula\\population_size_formula_data.txt", target_ratio=2.3)
    # file_path = r"D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\population_formula\\path.txt"
    # size, connections, points = read_data_from_file_connections(file_path)
    # generate_plot(size, connections, points)
