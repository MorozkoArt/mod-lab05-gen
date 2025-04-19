import os
import matplotlib.pyplot as plt
import matplotlib

matplotlib.rcParams['font.family'] = 'Arial'
def read_data(filename):
    num_lines=100
    with open(filename, 'r', encoding='utf-8') as file:
        lines = [line.strip().split() for line in file.readlines()[:num_lines] if line.strip()]
    return lines

def plot_data(data, title, save_path):
    labels = [item[0] for item in data]
    expected = [float(item[1].replace(',', '.')) for item in data]
    current = [float(item[2].replace(',', '.')) for item in data]
    
    x = range(len(labels))
    plt.figure(figsize=(15, 8))
    width = 0.45
    plt.bar([i - width/2 for i in x], expected, width, label='Ожидаемые', color='grey')
    plt.bar([i + width/2 for i in x], current, width, label='Текущие', color='purple')
    
    plt.title(title, fontsize=14)
    plt.xlabel('Параметры', fontsize=12)
    plt.ylabel('Частота', fontsize=12)
    plt.xticks(x, labels, rotation=90, fontsize=8)
    plt.legend()
    plt.tight_layout()
    plt.grid(True, axis='y', linestyle='--', alpha=0.7)
    plt.savefig(save_path, dpi=300, bbox_inches='tight')

bigrams_path = os.path.join(os.getcwd(), "Results", "data-gen-1.txt")
plot_bigrams = os.path.join(os.getcwd(), "Results", "gen-1.png")
bigrams = read_data(bigrams_path)
plot_data(bigrams, 'Распределения частот для биграмм', plot_bigrams)

words_path = os.path.join(os.getcwd(), "Results", "data-gen-2.txt")
plot_words = os.path.join(os.getcwd(), "Results", "gen-2.png")
words = read_data(words_path)
plot_data(words, 'Распределения частот для слов', plot_words)