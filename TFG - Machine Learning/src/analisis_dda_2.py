import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os
import matplotlib.ticker as mtick

sns.set_theme(style="whitegrid", context="paper", font_scale=1.2)

def main():
    file_name = os.path.join('data', 'validation_results.csv')
    
    if not os.path.exists(file_name):
        print(f"Error: No se ha encontrado el archivo {file_name}.")
        return

    df = pd.read_csv(file_name)
    df['Modo'] = df['DDA_Active'].apply(lambda x: 'DDA Activat' if x else 'Joc base (sense DDA)')

    print("="*60)
    print("ANÁLISIS DE COMPORTAMIENTO Y RITMO (FASE 2)")
    print("="*60)


    # 1. ANÁLISIS DE PERFILES Y VOLATILIDAD
    
    df_real_profiles = df[df['Active_Profile'] != 'Default'].copy()

    # Función para contar cuántas veces cambia el perfil en una sesión
    def count_profile_changes(group):
        group = group.sort_values('Room_Index')
        if len(group) < 2:
            return 0
        changes = (group['Active_Profile'] != group['Active_Profile'].shift()).sum()
        return max(0, changes - 1)

    # Calculamos los cambios por sesión y sacamos la media según el modo
    volatility_df = df_real_profiles.groupby(['Session_ID', 'Modo']).apply(count_profile_changes).reset_index(name='Profile_Changes')
    mean_volatility = volatility_df.groupby('Modo')['Profile_Changes'].mean()

    print("\nVOLATILIDAD DEL COMPORTAMIENTO (Cambios de Perfil por Partida)")
    for modo, media_cambios in mean_volatility.items():
        print(f"  - {modo}: {media_cambios:.2f} cambios de perfil de media")

    print("\nGenerando gráficas PNG...")

    # GRÁFICA 4: Distribución de Perfiles (Barras Apiladas al 100%)
    # Calculamos el porcentaje de cada perfil dentro de cada modo
    profile_counts = df_real_profiles.groupby(['Modo', 'Active_Profile']).size().reset_index(name='Count')
    profile_counts['Percentage'] = profile_counts.groupby('Modo')['Count'].transform(lambda x: (x / x.sum()) * 100)
    
    pivot_df = profile_counts.pivot(index='Modo', columns='Active_Profile', values='Percentage').fillna(0)

    profile_colors = sns.color_palette("Set2", n_colors=len(pivot_df.columns))

    fig, ax = plt.subplots(figsize=(9, 6))
    pivot_df.plot(kind='bar', stacked=True, color=profile_colors, ax=ax, edgecolor='white', width=0.6)

    ax.set_title("Distribució de Perfils Predits per la IA", fontsize=14, fontweight='bold')
    ax.set_xlabel("Mode de Joc", fontsize=12)
    ax.set_ylabel("Proporció (%)", fontsize=12)
    ax.yaxis.set_major_formatter(mtick.PercentFormatter())
    
    plt.legend(title='Perfil (IA)', bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.xticks(rotation=0)
    plt.tight_layout()
    plt.savefig("Grafica_4_Distribucion_Perfiles.png", dpi=300)
    plt.close()

    # GRÁFICA 5: Tiempo por Sala (Ritmo / Pacing)
    plt.figure(figsize=(9, 6))
    
    sns.violinplot(
        data=df, 
        x='Modo', 
        y='Time_In_Room', 
        palette=['#e74c3c', '#2ecc71'], 
        inner="quartile",
        cut=0
    )

    plt.title("Ritme de Joc: Distribució del Temps per Sala", fontsize=14, fontweight='bold')
    plt.xlabel("Mode de Joc", fontsize=12)
    plt.ylabel("Temps per netejar la sala (Segons)", fontsize=12)
    plt.tight_layout()
    plt.savefig("Grafica_5_Tiempo_Ritmo.png", dpi=300)
    plt.close()

    print("¡Gráficas Fase 2 exportadas con éxito! Revisa tu carpeta actual.")

if __name__ == "__main__":
    main()