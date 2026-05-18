import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os

sns.set_theme(style="whitegrid", context="paper", font_scale=1.2)

def main():
    file_name = os.path.join('data', 'validation_results.csv')

    if not os.path.exists(file_name):
        print(f"Archivo no encontrado: {file_name}")
        return
    
    df = pd.read_csv(file_name)

    df['Modo'] = df['DDA_Active'].apply(lambda x: 'DDA Activat' if x else 'Joc base (sense DDA)')

    df['Health_Percent'] = df['Remaining_Health_Percentage'] * 100

    print("="*50)
    print(" RESULTADOS DEL ANÁLISIS A/B (DDA) ")
    print("="*50)

    # Métricas globales
    wins = df[(df['Room_Index'] == 15) & (df['Room_Cleared'] == True)]['Session_ID'].unique()

    session_stats = df.groupby(['Session_ID', 'Modo']).agg(Max_Room=('Room_Index', 'max')).reset_index()
    
    session_stats['Won'] = session_stats['Session_ID'].isin(wins)
    
    summary = []
    modos = ['Joc base (sense DDA)', 'DDA Activat']
    
    for modo in modos:
        sesiones_modo = session_stats[session_stats['Modo'] == modo]
        total_sesiones = len(sesiones_modo)
        sesiones_ganadas = sesiones_modo['Won'].sum()
        win_rate = (sesiones_ganadas / total_sesiones) * 100 if total_sesiones > 0 else 0
        
        # Sala máxima media (solo para los que murieron)
        sesiones_perdidas = sesiones_modo[sesiones_modo['Won'] == False]
        avg_max_room_loss = sesiones_perdidas['Max_Room'].mean() if not sesiones_perdidas.empty else 0
        
        # Daño medio global
        avg_damage = df[df['Modo'] == modo]['Damage_Taken'].mean()

        summary.append({
            'Modo': modo,
            'Win Rate (%)': win_rate,
            'Avg Max Room (Defeats)': avg_max_room_loss,
            'Avg Damage per Room': avg_damage
        })

        print(f"\n {modo.upper()} (Total partides: {total_sesiones})")
        print(f"  - Taxa de Victòria: {win_rate:.1f}%")
        print(f"  - Sala màxima mitjana (en derrotes): {avg_max_room_loss:.1f}")
        print(f"  - Dany mitjà rebut per sala: {avg_damage:.2f}")

    print("\nGenerando gráficas PNG...")

    # GRÁFICA 1: Curva de Supervivencia (Salud Media)
    plt.figure(figsize=(10, 6))
    sns.lineplot(
        data=df, 
        x='Room_Index', 
        y='Health_Percent', 
        hue='Modo', 
        marker='o',
        linewidth=2,
        palette=['#e74c3c', '#2ecc71'] # Rojo para Juego Base, Verde para DDA
    )
    plt.title("Corba de Supervivència: Salut Mitjana per Sala", fontsize=14, fontweight='bold')
    plt.xlabel("Índex de la Sala", fontsize=12)
    plt.ylabel("Salut Restant Mitjana (%)", fontsize=12)
    plt.xticks(range(1, 16))
    plt.ylim(0, 105)
    plt.legend(title="Mode de Joc")
    plt.tight_layout()
    plt.savefig("Grafica_1_Supervivencia.png", dpi=300)
    plt.close()

    # GRÁFICA 2: Tasa de Victoria
    df_summary = pd.DataFrame(summary)
    plt.figure(figsize=(8, 6))
    ax = sns.barplot(
        data=df_summary, 
        x='Modo', 
        y='Win Rate (%)', 
        palette=['#e74c3c', '#2ecc71']
    )
    plt.title("Taxa de Victòria per Mode de Joc", fontsize=14, fontweight='bold')
    plt.xlabel("Mode", fontsize=12)
    plt.ylabel("Percentatge de Victòries (%)", fontsize=12)
    plt.ylim(0, 100)
    
    # Añadir los porcentajes encima de las barras
    for p in ax.patches:
        ax.annotate(f'{p.get_height():.1f}%', 
                    (p.get_x() + p.get_width() / 2., p.get_height()), 
                    ha='center', va='center', 
                    xytext=(0, 9), 
                    textcoords='offset points',
                    fontsize=12, fontweight='bold')

    plt.tight_layout()
    plt.savefig("Grafica_2_Tasa_Victoria.png", dpi=300)
    plt.close()

    # GRÁFICA 3: Daño Recibido por Sala
    plt.figure(figsize=(8, 6))
    sns.boxplot(
        data=df, 
        x='Modo', 
        y='Damage_Taken', 
        palette=['#e74c3c', '#2ecc71'],
        showmeans=True, 
        meanprops={"marker":"o", "markerfacecolor":"white", "markeredgecolor":"black", "markersize":"8"}
    )
    plt.title("Dispersió del Dany Rebut per Sala", fontsize=14, fontweight='bold')
    plt.xlabel("Mode de Joc", fontsize=12)
    plt.ylabel("Dany Rebut", fontsize=12)
    plt.tight_layout()
    plt.savefig("Grafica_3_Dano_Recibido.png", dpi=300)
    plt.close()

    print("¡Gráficas exportadas con éxito! Revisa tu carpeta actual.")

if __name__ == "__main__":
    main()