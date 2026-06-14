import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os

sns.set_theme(style="whitegrid", context="paper", font_scale=1.2)

def cargar_y_etiquetar_datos():
    path_dev = os.path.join('data', 'datos_dev.csv')
    path_testers = os.path.join('data', 'datos_testers.csv')

    if not os.path.exists(path_dev) or not os.path.exists(path_testers):
        print("Error: Asegúrate de tener 'datos_dev.csv' y 'datos_testers.csv' en la carpeta 'data'.")
        return None

    df_dev = pd.read_csv(path_dev)
    df_dev['Grup'] = 'Desenvolupador'

    df_testers = pd.read_csv(path_testers)
    df_testers['Grup'] = 'Testers'

    df_total = pd.concat([df_dev, df_testers], ignore_index=True)
    
    df_total['Modo'] = df_total['DDA_Active'].apply(lambda x: 'DDA Activat' if x else 'Joc Base (sense DDA)')
    
    return df_total

def main():
    print("="*60)
    print(" INICIANT ANÀLISI COMPARATIU (DEV vs TESTERS) ")
    print("="*60)

    df = cargar_y_etiquetar_datos()
    if df is None: return

    out_dir = "grafiques_comparatives"
    os.makedirs(out_dir, exist_ok=True)

    # ==========================================
    # GRÀFICA 1: PROGRÉS MITJÀ (Sala Màxima)
    # ==========================================
    print("-> Generant Gràfica 1: Progrés Mitjà...")
    
    session_progression = df.groupby(['Session_ID', 'Modo', 'Grup']).agg(Max_Room=('Room_Index', 'max')).reset_index()

    plt.figure(figsize=(9, 6))
    sns.boxplot(
        data=session_progression, 
        x='Grup', 
        y='Max_Room', 
        hue='Modo', 
        palette=['#3498db', '#e74c3c'],
        showmeans=True, 
        meanprops={"marker":"o", "markerfacecolor":"white", "markeredgecolor":"black", "markersize":"8"}
    )
    plt.title("Progrés Mitjà per Sessió (Sala Màxima Assolida)", fontsize=14, fontweight='bold')
    plt.xlabel("Grup d'Estudi", fontsize=12)
    plt.ylabel("Sala Màxima (Room Index)", fontsize=12)
    plt.legend(title='Mode de Joc')
    plt.tight_layout()
    plt.savefig(os.path.join(out_dir, "G1_Progres_Mitja.png"), dpi=300)
    plt.close()

    # ==========================================
    # GRÀFICA 2: PRESSIÓ DE L'ENTORN (Dany Rebut)
    # ==========================================
    print("-> Generant Gràfica 2: Dany Rebut...")
    
    plt.figure(figsize=(9, 6))
    sns.boxplot(
        data=df, 
        x='Grup', 
        y='Damage_Taken', 
        hue='Modo', 
        palette=['#3498db', '#e74c3c'],
        showfliers=False
    )
    plt.title("Impacte de la Generació Procedimental en el Dany Rebut", fontsize=14, fontweight='bold')
    plt.xlabel("Grup d'Estudi", fontsize=12)
    plt.ylabel("Dany Rebut per Sala", fontsize=12)
    plt.legend(title='Mode de Joc')
    plt.tight_layout()
    plt.savefig(os.path.join(out_dir, "G2_Dany_Rebut.png"), dpi=300)
    plt.close()

    # ==========================================
    # GRÀFICA 3: VOLATILITAT ESTRATÈGICA (Canvis de Perfil)
    # ==========================================
    print("-> Generant Gràfica 3: Volatilitat Estratègica...")
    
    def calculate_volatility_rate(group):
        group = group.sort_values('Room_Index')
        rooms_played = len(group)
        if rooms_played < 2: return 0
        changes = (group['Active_Profile'] != group['Active_Profile'].shift()).sum()
        actual_changes = max(0, changes - 1)
        # Normalizamos: Canvis / Sales jugades
        return actual_changes / rooms_played

    volatility = df[df['Active_Profile'] != 'Default'].groupby(['Session_ID', 'Modo', 'Grup']).apply(calculate_volatility_rate).reset_index(name='Volatility_Rate')

    plt.figure(figsize=(9, 6))
    sns.barplot(
        data=volatility, 
        x='Grup', 
        y='Volatility_Rate', 
        hue='Modo', 
        palette=['#3498db', '#e74c3c'],
        ci=None
    )
    plt.title("Taxa de Volatilitat Estratègica (Canvis d'Arquetip / Sala)", fontsize=14, fontweight='bold')
    plt.xlabel("Grup d'Estudi", fontsize=12)
    plt.ylabel("Ratio de Canvis per Sala", fontsize=12)
    plt.legend(title='Mode de Joc')
    plt.tight_layout()
    plt.savefig(os.path.join(out_dir, "G3_Volatilitat_Normalitzada.png"), dpi=300)
    plt.close()

    # ==========================================
    # GRÀFICA 4: RITME DE JOC (Temps per Sala)
    # ==========================================
    print("-> Generant Gràfica 4: Ritme de Joc...")
    
    plt.figure(figsize=(9, 6))
    sns.violinplot(
        data=df, 
        x='Grup', 
        y='Time_In_Room', 
        hue='Modo', 
        split=True,
        palette=['#3498db', '#e74c3c'],
        inner="quartile"
    )
    plt.title("Distribució del Temps de Supervivència per Sala", fontsize=14, fontweight='bold')
    plt.xlabel("Grup d'Estudi", fontsize=12)
    plt.ylabel("Temps a la Sala (segons)", fontsize=12)
    plt.legend(title='Mode de Joc', loc='upper left')
    plt.tight_layout()
    plt.savefig(os.path.join(out_dir, "G4_Temps_Sala.png"), dpi=300)
    plt.close()

    print("="*60)
    print("¡Gràfiques generades amb èxit a la carpeta 'grafiques_comparatives/'!")

if __name__ == "__main__":
    main()